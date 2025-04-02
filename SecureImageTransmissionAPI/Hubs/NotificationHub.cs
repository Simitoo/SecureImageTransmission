using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SecureImageTransmissionAPI.Interfaces;
using SecureImageTransmissionAPI.Models;

namespace SecureImageTransmissionAPI.Hubs
{
    public class NotificationHub : Hub<INotificationHub>
    {
        static int _clientCount;

        public override async Task OnConnectedAsync()
        {
            _clientCount++;
            await Clients.All.UpdateClientCount(_clientCount);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _clientCount--;
            await Clients.All.UpdateClientCount(_clientCount);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(Notification notification)
        {
            await Clients.Client(Context.ConnectionId).ReceiveNotification(notification);
        }

    }
}
