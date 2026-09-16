using MediatR;

namespace ToMainApi.Features.Events.Applications
{
    public record ApplicationApprovedEvent(int ApplicationId, int UserId) : INotification;
}
