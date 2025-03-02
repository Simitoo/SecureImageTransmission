using SecureImageTransmissionAPI.Models;

namespace SecureImageTransmissionAPI.Interfaces
{
    public interface IImageGenerator
    {
        ImageModel GenerateImage(int width, int height, string format);
    }
}
