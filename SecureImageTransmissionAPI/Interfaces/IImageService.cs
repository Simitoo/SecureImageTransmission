using SecureImageTransmissionAPI.Models;

namespace SecureImageTransmissionAPI.Interfaces
{
    public interface IImageService
    {
        ImageModel GenerateImage(int width, int height, string format);
    }
}
