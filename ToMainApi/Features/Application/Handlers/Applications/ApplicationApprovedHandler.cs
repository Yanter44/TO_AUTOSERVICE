using MediatR;
using ToMainApi.DbContext;
using ToMainApi.Features.Application.Events.Applications;
using ToMainApi.Interfaces;

namespace ToMainApi.Features.Application.Handlers.Applications
{
    public class ApplicationApprovedHandler : INotificationHandler<ApplicationApprovedEvent>
    {
        private readonly IApplicationNotificationService _applicationNotificationService;
        public ApplicationApprovedHandler(IApplicationNotificationService applicationNotificationService)
        {
            _applicationNotificationService = applicationNotificationService;
        }
        public async Task Handle(ApplicationApprovedEvent notification, CancellationToken ct)
        {
            await _applicationNotificationService.NotifyApplicationApproved(notification.ApplicationId, notification.UserId);
        }
    }
}
