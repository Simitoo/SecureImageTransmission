using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SecureImageTransmissionAPI.Common.Constants;
using SecureImageTransmissionAPI.Interfaces;

namespace SecureImageTransmissionAPI.Hubs
{
    [Authorize]
    public class ImageHub : Hub<IImageHub>
    {
        private readonly ILogger<ImageHub> _logger;
        private readonly IImageGenerationService _imageGenerationService;

        public ImageHub(IImageGenerationService imageGenerationService, ILogger<ImageHub> logger)
        {
            _imageGenerationService = imageGenerationService;
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var tokenFromQuery = httpContext?.Request.Query["access_token"];
            var tokenFromHeader = httpContext?.Request.Headers["Authorization"];

            _logger.LogInformation("🔍 Token from Query String: {TokenQuery}", tokenFromQuery);
            _logger.LogInformation("🔍 Token from Authorization Header: {TokenHeader}", tokenFromHeader);

            return base.OnConnectedAsync();
        }

        public async Task<string> GenerateImage(int width, int height, string format)
        {
            _logger.LogInformation("Image generation requested with width: {Width}, height: {Height}, format: {Format}", width, height, format);
            var result = _imageGenerationService.StartGenerating(width, height, format);
            if (!result.IsSuccess)
            {
                _logger.LogError("Error starting image generation: {Message}", result.Message);
                await Clients.Caller.ReceiveError(result.Message ?? "An unknown error occurred.");
                return result.Message ?? "An unknown error occurred.";
            }

            return Messages.ImageGenerationStarted;
        }

        public Task StopGeneratingImage()
        {
            _imageGenerationService.StopGenerating();
            return Task.CompletedTask;
        }
    }
}
