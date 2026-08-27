using MediatR;

namespace ToMainApi.Features.Application.Events.Applications
{
    public record ApplicationCreatedEvent(int ApplicationId, int UserId) : INotification;
}
