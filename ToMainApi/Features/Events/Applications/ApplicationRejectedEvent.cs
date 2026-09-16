using MediatR;

namespace ToMainApi.Features.Events.Applications
{
    public record ApplicationRejectedEvent(int ApplicationId, int UserId, string rejectReason) : INotification;
}
