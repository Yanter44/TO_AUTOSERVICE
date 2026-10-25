using ToMainApi.Common;
using ToMainApi.Models.Dtos.Notification;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Enums;

namespace ToMainApi.Interfaces
{
    public interface INotificationService
    {
        
        Task<ServiceResponse<List<NotificationDto>>> GetNotifications(int userId, int page, int pageSize);
        Task<ServiceResponse<bool>> MarkNotificationAsRead(UserContextDto userContext, int notificationId);
        Task<ServiceResponse<bool>> MarkAllMyNotificationsAsRead(UserContextDto userContext);
    }
}
