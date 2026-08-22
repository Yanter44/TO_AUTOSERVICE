namespace ToMainApi.Interfaces
{
    public interface INotificationSender
    {
        Task SomethingToAgents(string message);
    }
}
