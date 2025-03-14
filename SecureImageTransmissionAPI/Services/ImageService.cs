using Microsoft.Extensions.Options;
using SecureImageTransmissionAPI.Common;
using SecureImageTransmissionAPI.Common.Constants;
using SecureImageTransmissionAPI.Common.Options;
using SecureImageTransmissionAPI.Interfaces;
using SecureImageTransmissionAPI.Models;
using SkiaSharp;

namespace SecureImageTransmissionAPI.Services
{
    public class ImageService : IImageService
    {
        private readonly FormatOptions _imageFormatOptions;
        private readonly ImageSizeOptions _imageSizeOptions;

        public ImageService(IOptions<FormatOptions> formtaOptions, IOptions<ImageSizeOptions> sizeOptions)
        {
            _imageFormatOptions = formtaOptions.Value;
            _imageSizeOptions = sizeOptions.Value;
        }

        public Result<ImageModel> GenerateImage(int width, int height, string format)
        {
            if (!IsSupportedFormat(format.ToLower()))
            {
                return Result<ImageModel>.Failure(string.Format(Messages.UnsupportedImageFormat, format));
            }

            if (!IsWithinLimits(width, height))
            {
                return Result<ImageModel>.Failure(Messages.ImageSizeOutOfRange);
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

            return Result<ImageModel>.Success(new ImageModel(imageBytes, format));
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
                _ => throw new InvalidOperationException(string.Format(Messages.UnsupportedImageFormat, format))
            };
        }

        private bool IsSupportedFormat(string format)
        {
            return _imageFormatOptions.SupportedFormats.Contains(format);
        }

        private bool IsWithinLimits(int width, int height)
        {
            return width >= _imageSizeOptions.MinWidth && width <= _imageSizeOptions.MaxWidth &&
               height >= _imageSizeOptions.MinHeight && height <= _imageSizeOptions.MaxHeight;
        }
    }
}
