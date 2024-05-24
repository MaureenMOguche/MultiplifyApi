using Multiplify.Application.Dtos;
using Multiplify.Application.Responses;

namespace Multiplify.Application.Contracts.Services;
public interface IUserNotificationService
{
    Task<ApiResponse> GetNotifications(BaseQueryParams queryParams);
    Task<ApiResponse> GetNotification(int notificationId);
    Task<ApiResponse> MarkNotificationAsRead(int notificationId);
    Task<ApiResponse> MarkAllNotificationsAsRead();
    Task<ApiResponse> DeleteNotification(int notificationId);
    Task<ApiResponse> DeleteAllNotifications();

}
