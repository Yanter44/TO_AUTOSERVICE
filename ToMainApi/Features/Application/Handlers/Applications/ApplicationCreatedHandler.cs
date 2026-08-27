using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ToMainApi.DbContext;
using ToMainApi.Features.Application.Events.Applications;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Notification;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;
using ToMainApi.Services.Notifications;

namespace ToMainApi.Features.Application.Handlers.Applications
{
    public class ApplicationCreatedHandler : INotificationHandler<ApplicationCreatedEvent>
    {
        private readonly IApplicationNotificationService _applicationNotificationService;
        public ApplicationCreatedHandler(IApplicationNotificationService applicationNotificationService)
        {
            _applicationNotificationService = applicationNotificationService;
        }
        public async Task Handle(ApplicationCreatedEvent notification, CancellationToken ct)
        {
            await _applicationNotificationService.NotifyApplicationCreated(notification.ApplicationId,notification.UserId);
        }
    }
}
