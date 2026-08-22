using MediatR;
using ToMainApi.Features.Application.Events;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Notification;
using ToMainApi.Models.Enums;

namespace ToMainApi.Features.Application.Handlers
{
    public class ApplicationCreatedHandler : INotificationHandler<ApplicationCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        public ApplicationCreatedHandler(INotificationService notificationService,IUserService userService)
        {
            _notificationService = notificationService;
            _userService = userService;
        }
        public async Task Handle(ApplicationCreatedEvent notification, CancellationToken ct)
        {
            var existUser = await _userService.GetUserById(notification.UserId);
            await _notificationService.NotifyApplicationCreated(existUser.Data.UserId,existUser.Data.FIO,notification.ApplicationId);
        }
    }
}
