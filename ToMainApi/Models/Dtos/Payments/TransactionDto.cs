using ToMainApi.Models.Entities;

namespace ToMainApi.Models.Dtos.Payments
{
    public class TransactionDto
    {
        public string ExternalId { get; set; }
        public string AgentName { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
