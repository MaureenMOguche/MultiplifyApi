using Microsoft.AspNetCore.SignalR;
using Multiplify.Application.Contracts;
using Multiplify.Application.SignalR;
using Multiplify.Domain;

namespace Multiplify.Application.ServiceImplementations;
public class WebNotificationService(IHubContext<NotificationHub> hubContext) : IWebNotificationService
{
    public async Task SendNotificationToAllUsers(Notification notification)
    {
        await hubContext.Clients.All.SendAsync("ReceiveNotification", notification.Message);
    }

    public async Task SendNotificationToMultiple(List<Notification> notifications)
    {
        foreach (var notification in notifications)
            await hubContext.Clients.User(notification.UserId).SendAsync("ReceiveNotification", notification.Message);
    }

    public async Task SendNotificationToUser(Notification notification)
    {
        await hubContext.Clients.User(notification.UserId).SendAsync("ReceiveNotification", notification.Message);
    }
}
