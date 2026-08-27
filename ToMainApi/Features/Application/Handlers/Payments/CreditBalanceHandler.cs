using MediatR;
using ToMainApi.Features.Application.Events.Applications;
using ToMainApi.Features.Application.Events.Payments;
using ToMainApi.Interfaces;

namespace ToMainApi.Features.Application.Handlers.Payments
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
            await _paymentNotificationService.NotifyBalanceCredited(notification.agentId, notification.ammount);
        }
    }
}
