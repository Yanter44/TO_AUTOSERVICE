using MediatR;
using ToMainApi.Features.Application.Events.Payments;
using ToMainApi.Interfaces;

namespace ToMainApi.Features.Application.Handlers.Payments
{
    public class DebitBalanceHandler : INotificationHandler<DebitBalanceEvent>
    {
        private readonly IPaymentNotificationService _paymentNotificationService;
        public DebitBalanceHandler(IPaymentNotificationService paymentNotificationService)
        {
            _paymentNotificationService = paymentNotificationService;
        }
        public async Task Handle(DebitBalanceEvent notification, CancellationToken ct)
        {
            await _paymentNotificationService.NotifyBalanceDebited(notification.agentId, notification.ammount);
        }
    }
}
