using MediatR;

namespace ToMainApi.Features.Application.Events.Applications
{
    public record ApplicationProcessingErrorEvent(int ApplicationId, int UserId,string ErrorMessage,string ErrorDetails = null) : INotification;
}
