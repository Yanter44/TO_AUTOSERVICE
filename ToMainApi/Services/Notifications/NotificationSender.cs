using Microsoft.AspNetCore.SignalR;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Notification;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services.Notifications
{
    public class NotificationSender : INotificationsSender
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationSender(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;            
        }
        public async Task SendToUser(int userId, string notificationEvent, NotificationDto notification)
        {
            await _hubContext.Clients
                .User(userId.ToString())
                .SendAsync(notificationEvent.ToString(), notification);
        }
        public async Task SendToUsers(List<int> userIds, string notificationEvent, List<NotificationDto> notifications)
        {
            if (!userIds.Any() || !notifications.Any()) return;

            await _hubContext.Clients
                .Users(userIds.Select(id => id.ToString()).ToList())
                .SendAsync(notificationEvent.ToString(), notifications);
        }

        public async Task SendToGroup(string groupName, string notificationEvent, NotificationDto notification)
        {
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync(notificationEvent.ToString(), notification);
        }

        public async Task SendToGroups(string[] groupNames, string notificationEvent, NotificationDto notification)
        {
            if (!groupNames.Any()) return;

            var tasks = groupNames.Select(groupName =>
                _hubContext.Clients.Group(groupName).SendAsync(notificationEvent.ToString(), notification)
            );

            await Task.WhenAll(tasks);
        }
    }
}
