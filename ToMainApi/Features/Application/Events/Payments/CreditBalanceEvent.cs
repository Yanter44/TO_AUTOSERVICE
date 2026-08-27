using MediatR;

namespace ToMainApi.Features.Application.Events.Payments
{
    public record class CreditBalanceEvent(int agentId, decimal ammount) : INotification;
}
