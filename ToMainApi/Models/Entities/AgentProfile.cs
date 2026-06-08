namespace ToMainApi.Models.Entities
{
    public class AgentProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Wallet Wallet { get; set; }
    }
}
