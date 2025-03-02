using SecureImageTransmissionAPI.Models;

namespace SecureImageTransmissionAPI.Interfaces
{
    public interface INotificationHub
    {
        Task ReceiveNotification(Notification notification);
        Task UpdateClientCount(int count);
    }
}
