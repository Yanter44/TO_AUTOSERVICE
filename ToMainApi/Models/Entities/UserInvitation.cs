using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Entities
{
    public class UserInvitation
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public Role RoleType { get; set; }
        public string Token { get; set; }        
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }

        public int? BranchId { get; set; }
        public AgentBranch? Branch { get; set; }
    }
}
