using MediatR;

namespace ToMainApi.Features.Application.Events.Applications
{
    public record ApplicationApprovedEvent(int ApplicationId, int UserId) : INotification;
}
