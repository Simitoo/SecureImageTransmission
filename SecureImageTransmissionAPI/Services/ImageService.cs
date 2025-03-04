using SecureImageTransmissionAPI.Interfaces;
using SecureImageTransmissionAPI.Models;
using SkiaSharp;

namespace SecureImageTransmissionAPI.Services
{
    public class ImageService : IImageService
    {
        private readonly string[] _supportedFormats;

        public ImageService(IConfiguration configuration)
        {
            _supportedFormats = configuration.GetSection("ImageGeneration:SupportedFormats").Get<string[]>() ?? Array.Empty<string>();
        }

        public ImageModel GenerateImage(int width, int height, string format)
        {

            if (!_supportedFormats.Contains(format.ToLower()))
            {
                throw new InvalidOperationException($"{format} not supported");
            }

            SKBitmap bitmap = new(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul));
            SKCanvas canvas = new(bitmap);

            canvas.Clear(SKColors.White);

            SKPaint paint = new()
            {
                Color = RandomColorGeneration(),
                Style = SKPaintStyle.Fill
            };

            SKShader shader = SKShader.CreateLinearGradient(
                new SKPoint(0, 0),
                new SKPoint(width, height),
                [RandomColorGeneration(), RandomColorGeneration(), RandomColorGeneration()],
                null,
                SKShaderTileMode.Clamp
            );

            paint.Shader = shader;
            canvas.DrawRect(0, 0, width, height, paint);

            byte[] imageBytes = EncodeImage(bitmap, ImageFormat(format));

            return new ImageModel(imageBytes, format);
        }

        private static SKColor RandomColorGeneration()
        {
            Random random = new();
            var randomColor = new SKColor(
                (byte)random.Next(0, 256),
                (byte)random.Next(0, 256),
                (byte)random.Next(0, 256)
            );

            return randomColor;
        }

        private static byte[] EncodeImage(SKBitmap bitmap, SKEncodedImageFormat format)
        {
            using SKImage image = SKImage.FromBitmap(bitmap);
            using SKData data = image.Encode(format, 100); // 100% quality
            return data.ToArray();
        }

        private static SKEncodedImageFormat ImageFormat(string format)
        {
            return format.ToLower() switch
            {
                "png" => SKEncodedImageFormat.Png,
                "jpeg" => SKEncodedImageFormat.Jpeg,
                _ => throw new InvalidOperationException("Format not supported")
            };
        }
    }
}
