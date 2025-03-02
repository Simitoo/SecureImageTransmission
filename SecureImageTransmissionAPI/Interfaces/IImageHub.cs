using SecureImageTransmissionAPI.Models;

namespace SecureImageTransmissionAPI.Interfaces
{
    public interface IImageHub
    {
        Task ReceiveImage(ImageModel image);
    }
}
