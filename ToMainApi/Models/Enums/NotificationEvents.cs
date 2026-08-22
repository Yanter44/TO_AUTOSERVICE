namespace ToMainApi.Models.Enums
{
    public class NotificationEvents
    {
        public const string ApplicationFinished = nameof(ApplicationFinished);
        public const string BalanceCredited = nameof(BalanceCredited); 
        public const string BalanceDebited = nameof(BalanceDebited);
        public const string ApplicationSendToModeration = nameof(ApplicationSendToModeration);
        public const string ApplicationRejected = nameof(ApplicationRejected);
        public const string AgentCreateApplication = nameof(AgentCreateApplication);

    }
}
