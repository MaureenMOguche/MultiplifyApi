using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Multiplify.Application.Contracts.Repository;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.Dtos;
using Multiplify.Application.Responses;
using Multiplify.Application.ServiceImplementations.Helpers;
using Multiplify.Domain;

namespace Multiplify.Application.ServiceImplementations;
public class UserNotificationService(IUnitOfWork db) : IUserNotificationService
{
    private readonly UserPrincipal? currentUser = UserHelper.CurrentUser();
    public Task<ApiResponse> DeleteAllNotifications()
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse> DeleteNotification(int notificationId)
    {
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "User is not authorized");

        var notification = await db.GetRepository<Notification>().GetAsync(x => x.Id == notificationId && x.UserId == currentUser.Id, true).FirstOrDefaultAsync();

        if (notification == null) return ApiResponse.Failure(StatusCodes.Status404NotFound, "Notification not found");
        db.GetRepository<Notification>().Delete(notification);
        await db.SaveChangesAsync();

        return ApiResponse<Notification>.Success(notification, "Successfully deleted notification");
    }

    public async Task<ApiResponse> GetNotification(int notificationId)
    {
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "User is not authorized");

        var notification = await db.GetRepository<Notification>().GetAsync(x => x.Id == notificationId && x.UserId == currentUser.Id, true).FirstOrDefaultAsync();

        if (notification == null) return ApiResponse.Failure(StatusCodes.Status404NotFound, "Notification not found");
        notification.IsRead = true;
        db.GetRepository<Notification>().Update(notification);
        await db.SaveChangesAsync();

        return ApiResponse<Notification>.Success(notification, "Successfully retrieved notification");
    }

    public async Task<ApiResponse> GetNotifications(BaseQueryParams queryParams)
    {
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "User is not authorized");
        var notifications = db.GetRepository<Notification>().GetAsync(x => x.UserId == currentUser.Id);

        if (!string.IsNullOrEmpty(queryParams.Search))
            notifications = notifications.Where(x => x.Message.Contains(queryParams.Search));

        notifications = notifications.OrderByDescending(x => x.CreatedAt);

        var paginate = notifications.Paginate(queryParams.PageNumber, queryParams.PageSize);

        return ApiResponse<Paginated<Notification>>.Success(paginate, "Successfully retrieved notifications");
    }

    public Task<ApiResponse> MarkAllNotificationsAsRead()
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse> MarkNotificationAsRead(int notificationId)
    {
        if (currentUser == null) return ApiResponse.Failure(StatusCodes.Status401Unauthorized, "User is not authorized");

        var notification = await db.GetRepository<Notification>().GetAsync(x => x.Id == notificationId && x.UserId == currentUser.Id, true).FirstOrDefaultAsync();

        if (notification == null) return ApiResponse.Failure(StatusCodes.Status404NotFound, "Notification not found");
        notification.IsRead = true;
        db.GetRepository<Notification>().Update(notification);
        await db.SaveChangesAsync();

        return ApiResponse<Notification>.Success(notification, "Successfully marked notification as read");
    }
}
