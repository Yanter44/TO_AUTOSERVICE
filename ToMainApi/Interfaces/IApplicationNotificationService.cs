namespace ToMainApi.Interfaces
{
    public interface IApplicationNotificationService
    {
        Task NotifyApplicationProcessingError(int applicationId, int agentId, string errorMessage);
        Task NotifyApplicationCreated(int applicationId, int agentId);
        Task NotifyApplicationApproved(int applicationId, int agentId);
        Task NotifyApplicationRejected(int applicationId, int agentId);
    }
}
