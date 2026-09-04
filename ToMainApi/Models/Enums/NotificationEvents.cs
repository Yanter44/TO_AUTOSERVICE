namespace ToMainApi.Models.Enums
{
    public class NotificationEvents
    {
        // === APPLICATIONS ===
        public const string ApplicationSendToModeration = nameof(ApplicationSendToModeration);
        public const string ApplicationRejected = nameof(ApplicationRejected);
        public const string ApplicationCreated = nameof(ApplicationCreated);
        public const string ApplicationApproved = nameof(ApplicationApproved);
        public const string ApplicationProcessingError = nameof(ApplicationProcessingError);

        // === PAYMENTS ===
        public const string BalanceCredited = nameof(BalanceCredited); 
        public const string BalanceDebited = nameof(BalanceDebited);
    }
}
