using MediatR;
using ToMainApi.Features.Application.Events.Applications;
using ToMainApi.Interfaces;

namespace ToMainApi.Features.Application.Handlers.Applications
{
    public class ApplicationRejectedHandler : INotificationHandler<ApplicationRejectedEvent>
    {
        private readonly IApplicationNotificationService _applicationNotificationService;
        public ApplicationRejectedHandler(IApplicationNotificationService applicationNotificationService)
        {
            _applicationNotificationService = applicationNotificationService;
        }
        public async Task Handle(ApplicationRejectedEvent notification, CancellationToken ct)
        {
            await _applicationNotificationService.NotifyApplicationRejected(notification.ApplicationId, notification.UserId);
        }
    }
}
