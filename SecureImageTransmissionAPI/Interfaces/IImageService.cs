using SecureImageTransmissionAPI.Common;
using SecureImageTransmissionAPI.Models;

namespace SecureImageTransmissionAPI.Interfaces
{
    public interface IImageService
    {
        Result<ImageModel> GenerateImage(int width, int height, string format);
    }
}
