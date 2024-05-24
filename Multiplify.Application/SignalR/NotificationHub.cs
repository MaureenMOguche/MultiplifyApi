using Microsoft.AspNetCore.SignalR;
using Multiplify.Domain;

namespace Multiplify.Application.SignalR;
public class NotificationHub : Hub
{
    public async Task SendNotification(Notification notification)
    {
        await Clients.User(notification.UserId).SendAsync("ReceiveNotification", notification.Message);
    }
}
