namespace ToMainApi.Interfaces
{
    public interface IPaymentNotificationService
    {
        Task NotifyBalanceCredited(int agentId, decimal amount);
        Task NotifyBalanceDebited(int agentId, decimal amount);
    }
}
