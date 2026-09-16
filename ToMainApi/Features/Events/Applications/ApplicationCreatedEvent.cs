using MediatR;

namespace ToMainApi.Features.Events.Applications
{
    public record ApplicationCreatedEvent(int ApplicationId, int UserId) : INotification;
}
