namespace ToMainApi.Models.Dtos.Agent
{
    public class AgentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FIO { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public decimal DebtLimit { get; set; }
    }
}
