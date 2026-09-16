using MediatR;
using ToMainApi.Features.Events.Applications;
using ToMainApi.Interfaces;

namespace ToMainApi.Features.Handlers.Applications
{
    public class ApplicationProcessingErrorHandler : INotificationHandler<ApplicationProcessingErrorEvent>
    {
        private readonly IApplicationNotificationService _applicationNotificationService;
        public ApplicationProcessingErrorHandler(IApplicationNotificationService applicationNotificationService)
        {
            _applicationNotificationService = applicationNotificationService;
        }
        public async Task Handle(ApplicationProcessingErrorEvent notification, CancellationToken ct)
        {
            await _applicationNotificationService.NotifyApplicationProcessingError(notification.ApplicationId, notification.UserId, notification.ErrorMessage);
        }
    }
}
