using ToMainApi.Common;
using ToMainApi.Models.Dtos.Notification;

namespace ToMainApi.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResponse<NotificationDto>> CreateNewNotification(CreateNotificationRequest model);
        Task<ServiceResponse<List<NotificationDto>>> GetNotifications(int userId, int page, int pageSize);
        Task NotifyApplicationCreated(int agentId, string agentName, int applicationId);
    }
}
