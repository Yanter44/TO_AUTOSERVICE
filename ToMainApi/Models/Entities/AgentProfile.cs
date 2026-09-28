namespace ToMainApi.Models.Entities
{
    public class AgentProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Wallet Wallet { get; set; }
        public int? ParentAgentId { get; set; }
        public AgentProfile? ParentAgent { get; set; }
        public ICollection<AgentProfile> Children { get; set; }
        public string Path { get; set; } = null!;
        public int? BranchId { get; set; }
        public AgentBranch? Branch { get; set; }
        public ICollection<AgentBranch> OwnedBranches { get; set; }
    }
}
