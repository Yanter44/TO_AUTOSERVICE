using ToMainApi.Models.Dtos.Notification;
using ToMainApi.Models.Enums;

namespace ToMainApi.Interfaces
{
    public interface INotificationsSender
    {
        Task SendToUser(int userId, string notificationEvent, NotificationDto notification);
        Task SendToUsers(List<int> userIds, string notificationEvent, List<NotificationDto> notifications);
        Task SendToGroup(string groupName, string notificationEvent, NotificationDto notification);
        Task SendToGroups(string[] groupNames, string notificationEvent, NotificationDto notification);
    }
}
