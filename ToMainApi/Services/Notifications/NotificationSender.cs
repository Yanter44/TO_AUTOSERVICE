using Microsoft.AspNetCore.SignalR;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Application;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services.Notifications
{
    public class NotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationHub> _notificationHub;

        public NotificationSender(IHubContext<NotificationHub> notificationHub)
        {
            _notificationHub = notificationHub;
        }
        public Task ApplicationCreated(ApplicationDto dto)
        {
            return _notificationHub.Clients.Group(NotificationGroupTypes.Admins)
                .SendAsync(NotificationEvents.ApplicationFinished, dto);
        }
        
        public Task SomethingToAgents(string message)
        {
            return _notificationHub.Clients.Group(NotificationGroupTypes.Agents).SendAsync(NotificationEvents.ApplicationFinished, message);
        }

    }
}
