namespace ToMainApi.Models.Dtos.Payments
{
    public class CreditRequest
    {
        public int AgentId { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public Guid IdempotencyKey { get; set; }
    }
}
