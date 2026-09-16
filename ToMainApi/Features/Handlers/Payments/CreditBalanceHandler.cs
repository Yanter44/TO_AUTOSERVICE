using MediatR;
using ToMainApi.Features.Events.Payments;
using ToMainApi.Interfaces;

namespace ToMainApi.Features.Handlers.Payments
{
    public class CreditBalanceHandler : INotificationHandler<CreditBalanceEvent>
    {
        private readonly IPaymentNotificationService _paymentNotificationService;
        public CreditBalanceHandler(IPaymentNotificationService paymentNotificationService)
        {
            _paymentNotificationService = paymentNotificationService;
        }
        public async Task Handle(CreditBalanceEvent notification, CancellationToken ct)
        {
            await _paymentNotificationService.NotifyBalanceCredited(notification.userId, notification.ammount);
        }
    }
}
