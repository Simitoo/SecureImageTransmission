using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;
using System.Text.Json;

namespace SecureImageTransmissionClient.Services
{
    public class SignalRService : IAsyncDisposable
    {
        private readonly HubConnection _imageHubConnection;
        private readonly HubConnection _notificationHubConnection;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "http://localhost:8081";

        public event Action<string>? OnImageReceived;
        public event Action<string>? OnErrorReceived;
        public event Action<int>? OnClientCountChanged;

        public SignalRService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _imageHubConnection = new HubConnectionBuilder()
                .WithUrl($"{_apiUrl}/imagehub", options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var token = await GetAccessTokenAsync();
                        if(string.IsNullOrEmpty(token))
                        {
                            Console.WriteLine("Failed to retrieve access token for SignalR.");
                        }
                        else
                        {
                            Console.WriteLine($"Token retrieved: {token.Substring(0, 10)}...");
                        }
                        return token;
                    };
                })
                .WithAutomaticReconnect()
                .Build();

            _notificationHubConnection = new HubConnectionBuilder()
                .WithUrl($"{_apiUrl}/notifications")
                .WithAutomaticReconnect()
                .Build();

            _imageHubConnection.On<string>("ReceiveImage", (image) =>
            {
                OnImageReceived?.Invoke(image);
            });

            _imageHubConnection.On<string>("ReceiveError", (error) =>
            {
                Console.WriteLine($"SignalR received error: {error}");
                OnErrorReceived?.Invoke(error);
            });

            _notificationHubConnection.On<int>("UpdateClientCount", (count) =>
            {
                OnClientCountChanged?.Invoke(count);
            });
        }

        private async Task<string?> GetAccessTokenAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>($"{_apiUrl}/api/token");
                if (response.TryGetProperty("token", out var tokenElement))
                {
                    var token = tokenElement.GetString();
                    Console.WriteLine($"Access token received: {token}");
                    return token;
                }
                else
                {
                    Console.WriteLine("Access token not found in response.");
                    return null;
                }
                //return response.GetProperty("token").GetString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting access token: {ex.Message}");
                return null;
            }

        }

        public async Task StartImageHubConnectionAsync()
        {
            if (_imageHubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _imageHubConnection.StartAsync();
                    Console.WriteLine("ImageHub connection started successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error starting ImageHub connection: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("ImageHub connection is already started.");
            }
        }

        public async Task StartNotificationHubConnectionAsync()
        {
            if (_notificationHubConnection.State == HubConnectionState.Disconnected)
            {
                await _notificationHubConnection.StartAsync();
            }
        }

        public async Task StopImageHubConnectionAsync()
        {
            if (_imageHubConnection.State == HubConnectionState.Connected)
            {
                await _imageHubConnection.StopAsync();
            }
        }

        public async Task StopNotificationHubConnectionAsync()
        {
            if (_notificationHubConnection.State == HubConnectionState.Connected)
            {
                await _notificationHubConnection.StopAsync();
            }
        }

        public async Task StartGenerateImageRequest(int width, int height, string format)
        {
            if (_imageHubConnection.State == HubConnectionState.Connected)
            {
                await _imageHubConnection.SendAsync("GenerateImage", width, height, format);
            }
        }

        public async Task StopGenerateImageRequest()
        {
            if (_imageHubConnection.State == HubConnectionState.Connected)
            {
                await _imageHubConnection.SendAsync("StopGeneratingImage");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _imageHubConnection.DisposeAsync();
            await _notificationHubConnection.DisposeAsync();
        }
    }
}
