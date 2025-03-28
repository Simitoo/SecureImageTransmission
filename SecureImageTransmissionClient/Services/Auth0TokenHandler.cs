
using SecureImageTransmissionClient.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SecureImageTransmissionClient.Services
{
    public class Auth0TokenHandler : DelegatingHandler
    {
        private readonly AuthSettings _authSettings;

        public Auth0TokenHandler(AuthSettings authSettings)
        {
            _authSettings = authSettings;
        }    

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }

        public async Task<string> GetAccessToken()
        {
            using var client = new HttpClient();

            var response = await client.PostAsync(
                $"https://{_authSettings.Domain}/oauth/token",
                new FormUrlEncodedContent(
                [
                    new("grant_type", "client_credentials"),
                    new("client_id", _authSettings.ClientId),
                    new("client_secret", _authSettings.ClientSecret),
                    new("audience", _authSettings.Audience),
                    new("scope", "read:images")
                ]));

            var content = await response.Content.ReadFromJsonAsync<Auth0TokenResponse>();
            return content!.AccessToken;
        }

        private record Auth0TokenResponse(string AccessToken);
    }
}
