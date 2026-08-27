using MediatR;

namespace ToMainApi.Features.Application.Events.Payments
{
    public record class DebitBalanceEvent(int agentId, decimal ammount) : INotification;
}
