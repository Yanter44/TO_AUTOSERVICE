using MediatR;

namespace ToMainApi.Features.Application.Events
{
    public record ApplicationCreatedEvent(int ApplicationId, int UserId) : INotification;
}
