using Microsoft.AspNetCore.SignalR;
using SecureImageTransmissionAPI.Common.Constants;
using SecureImageTransmissionAPI.Interfaces;

namespace SecureImageTransmissionAPI.Hubs
{
    public class ImageHub : Hub<IImageHub>
    {
        private readonly ILogger<ImageHub> _logger;
        private readonly IImageGenerationService _imageGenerationService;

        public ImageHub(IImageGenerationService imageGenerationService, ILogger<ImageHub> logger)
        {
            _imageGenerationService = imageGenerationService;
            _logger = logger;
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
