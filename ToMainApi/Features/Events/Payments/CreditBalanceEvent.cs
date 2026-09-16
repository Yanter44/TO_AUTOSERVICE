using MediatR;

namespace ToMainApi.Features.Events.Payments
{
    public record class CreditBalanceEvent(int userId, decimal ammount) : INotification;
}
