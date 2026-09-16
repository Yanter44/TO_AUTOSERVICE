using MediatR;

namespace ToMainApi.Features.Events.Payments
{
    public record class DebitBalanceEvent(int userId, decimal ammount) : INotification;
}
