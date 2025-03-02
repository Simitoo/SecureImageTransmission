using Microsoft.AspNetCore.SignalR;
using SecureImageTransmissionAPI.Interfaces;
using SecureImageTransmissionAPI.Models;
using SecureImageTransmissionAPI.Services;

namespace SecureImageTransmissionAPI.Hubs
{
    public class ImageHub : Hub<IImageHub>
    {
        private readonly ImageGeneratorService _imageGenerator;
        private bool _isGenerating = true;

        public ImageHub(ImageGeneratorService imageGenerator)
        {
            _imageGenerator = imageGenerator;
        }

        public async Task GenerateImage(int width, int height, string format)
        {
            int milliseconds = 5000;

            while (_isGenerating)
            {
                ImageModel image = _imageGenerator.GenerateImage(width, height, format);
                await Clients.Caller.ReceiveImage(image);
                await Task.Delay(milliseconds);
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _isGenerating = false;
            return base.OnDisconnectedAsync(exception);
        }
    }
}
