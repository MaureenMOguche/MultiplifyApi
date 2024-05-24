using Multiplify.Domain;

namespace Multiplify.Application.Contracts;
public interface IWebNotificationService
{
    Task SendNotificationToUser(Notification notification);
    Task SendNotificationToMultiple(List<Notification> notifications);
    Task SendNotificationToAllUsers(Notification notification);
}
