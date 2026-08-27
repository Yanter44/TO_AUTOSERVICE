using ToMainApi.Common;
using ToMainApi.Models.Dtos.Notification;
using ToMainApi.Models.Enums;

namespace ToMainApi.Interfaces
{
    public interface INotificationService
    {
        
        Task<ServiceResponse<List<NotificationDto>>> GetNotifications(int userId, int page, int pageSize);
    }
}
