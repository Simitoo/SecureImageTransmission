using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SecureImageTransmissionAPI
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TokenController> _logger;

        public TokenController(HttpClient httpClient, ILogger<TokenController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IActionResult> GetToken()
        {

            var requestData = new
            {
                client_id = "pFt94ChIz1mA37EIL0F2UbNPLoe5nqy1",
                client_secret = "3Xmt8xK_lsswUnOWC9q2WotBbiXk3rz1x0QBnxIBdZaGs1TftaudE67BzbVsbRbr", //The Client Secret is not base64 encoded.
                audience = "https://secureimageapi.com",
                grant_type = "client_credentials"
            };

            _logger.LogDebug($"Request payload: {JsonSerializer.Serialize(requestData)}");
            var respone = await _httpClient.PostAsJsonAsync("https://dev-uby1yrtmiyahcuyc.eu.auth0.com/oauth/token", requestData);

            if (!respone.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to get access token. :( -> {respone.StatusCode}");
                return Unauthorized("Failed to get access token. :(");
            }

            var jsonResponse = await respone.Content.ReadFromJsonAsync<JsonElement>();
            var accessToken = jsonResponse.GetProperty("access_token").GetString();

            _logger.LogInformation("Access token retrieved successfully.");
            return Ok(new {token = accessToken});
        }
    }
}
