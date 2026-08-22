using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Dtos.Agent
{
    public class AgentTransactionDto
    {
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public  string TransactionStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
