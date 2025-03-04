using SecureImageTransmissionAPI.Models;

namespace SecureImageTransmissionAPI.Interfaces
{
    public interface IImageHub
    {
        Task ReceiveImage(string imageSrc);

        Task GenerateImage(int width, int height, string format);

        Task StopGeneratingImage();
    }
}
