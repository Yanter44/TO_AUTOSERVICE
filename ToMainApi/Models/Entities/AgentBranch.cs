namespace ToMainApi.Models.Entities
{
    public class AgentBranch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Fee { get; set; }
        public int OwnerAgentId { get; set; }
        public AgentProfile Owner { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
