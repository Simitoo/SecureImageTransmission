
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace SecureImageTransmissionClient.Services
{
    public class SignalRService : IAsyncDisposable
    {
        private readonly HubConnection _imageHubConnection;
        private readonly HubConnection _notificationHubConnection;

        public event Action<string>? OnImageReceived;
        public event Action<int>? OnClientCountChanged;

        public SignalRService(NavigationManager navigation)
        {
            _imageHubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:8081/imagehub")
                .Build();

            _notificationHubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:8081/notifications")
                .Build();

            _imageHubConnection.On<string>("ReceiveImage", (image) =>
            {
                OnImageReceived?.Invoke(image);
            });

            _notificationHubConnection.On<int>("UpdateClientCount", (count) =>
            {
                OnClientCountChanged?.Invoke(count);
            });

        }

        public async Task StartImageHubConnectionAsync()
        {
            if(_imageHubConnection.State == HubConnectionState.Disconnected)
            {
                await _imageHubConnection.StartAsync();
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
            if(_imageHubConnection.State == HubConnectionState.Connected)
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
