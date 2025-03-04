using Microsoft.AspNetCore.SignalR;
using SecureImageTransmissionAPI.Interfaces;

namespace SecureImageTransmissionAPI.Hubs
{
    public class ImageHub : Hub<IImageHub>
    {
        private readonly IImageGenerationService _imageGenerationService;

        public ImageHub(IImageGenerationService imageGenerationService)
        {
            _imageGenerationService = imageGenerationService;
        }

        public Task GenerateImage(int width, int height, string format)
        {
            _imageGenerationService.StartGenerating(width, height, format);
            return Task.CompletedTask;
        }

        public Task StopGeneratingImage()
        {
            _imageGenerationService.StopGenerating();
            return Task.CompletedTask;
        }
    }
}
