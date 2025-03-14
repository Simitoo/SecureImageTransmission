using Microsoft.AspNetCore.SignalR;
using SecureImageTransmissionAPI.Common;
using SecureImageTransmissionAPI.Common.Constants;
using SecureImageTransmissionAPI.Hubs;
using SecureImageTransmissionAPI.Interfaces;

namespace SecureImageTransmissionAPI.Services
{
    public class ImageGenerationService : BackgroundService, IImageGenerationService
    {
        private readonly ILogger<ImageGenerationService> _logger;
        private static readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
        private readonly IImageService _imageService;
        private readonly IHubContext<ImageHub, IImageHub> _hubContext;
        private CancellationTokenSource _tokenSource = new();
        private int _width;
        private int _height;
        private string _format = string.Empty;

        public ImageGenerationService(IImageService imageService, IHubContext<ImageHub, IImageHub> hubContext, ILogger<ImageGenerationService> logger)
        {
            _imageService = imageService;
            _hubContext = hubContext;
            _logger = logger;
        }

        public Result<string> StartGenerating(int width, int height, string format)
        {
            StopGenerating();

            _width = width;
            _height = height;
            _format = format;
            _tokenSource = new CancellationTokenSource();

            _ = GenerateImagesAsync(_tokenSource.Token);
            return Result<string>.Success(Messages.ImageGenerationStarted);
        }

        public void StopGenerating()
        {
            _tokenSource.Cancel();
        }

        private async Task GenerateImagesAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Image generation started at {Time}", DateTime.UtcNow);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var imageResult = _imageService.GenerateImage(_width, _height, _format);
                    if (!imageResult.IsSuccess || imageResult.Value == null)
                    {
                        _logger.LogError("Error generating image: {Message}, passed format: {Format}", imageResult.Message, _format);
                        await _hubContext.Clients.All.ReceiveError(imageResult.Message ?? "Unknown error occurred.");
                        return;
                    }

                    _logger.LogInformation("Image generated successfully at {Time}: Width={Width}, Height={Height}, Format={Format}", DateTime.UtcNow, _width, _height, _format);
                    await _hubContext.Clients.All.ReceiveImage(imageResult.Value.ToBase64());
                    await Task.Delay(_interval, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Image generation cancelled at {Time}", DateTime.UtcNow);
                await _hubContext.Clients.All.ReceiveError(Messages.ImageGenerationCancelled);
                return;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
