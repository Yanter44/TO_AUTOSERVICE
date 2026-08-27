using MediatR;

namespace ToMainApi.Features.Application.Events.Applications
{
    public record ApplicationRejectedEvent(int ApplicationId, int UserId, string rejectReason) : INotification;
}
