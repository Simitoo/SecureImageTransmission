using Microsoft.AspNetCore.SignalR;
using SecureImageTransmissionAPI.Hubs;
using SecureImageTransmissionAPI.Interfaces;

namespace SecureImageTransmissionAPI.Services
{
    public class ImageGenerationService : BackgroundService, IImageGenerationService
    {
        private static readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
        private readonly IImageService _imageService;
        private readonly IHubContext<ImageHub, IImageHub> _hubContext;
        private CancellationTokenSource _tokenSource = new();
        private int _width;
        private int _height;
        private string _format = string.Empty;

        public ImageGenerationService(IImageService imageService, IHubContext<ImageHub, IImageHub> hubContext)
        {
            _imageService = imageService;
            _hubContext = hubContext;
        }

        public void StartGenerating(int width, int height, string format)
        {
            StopGenerating();

            _width = width;
            _height = height;
            _format = format;
            _tokenSource = new CancellationTokenSource();

            _ = GenerateImagesAsync(_tokenSource.Token);
        }

        public void StopGenerating()
        {
            _tokenSource.Cancel();
        }

        private async Task GenerateImagesAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var image = _imageService.GenerateImage(_width, _height, _format);
                    await _hubContext.Clients.All.ReceiveImage(image.ToBase64());

                    await Task.Delay(_interval, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                // Ignore
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
