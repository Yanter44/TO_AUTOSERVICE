using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public Guid IdempotencyKey { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
