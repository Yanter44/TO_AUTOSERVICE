namespace ToMainApi.Models.Entities
{
    public class UserBlocks
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int BlockedByUserId { get; set; }
        public User BlockedBy { get; set; }

        public string Reason { get; set; }
        public DateTime BlockedAt { get; set; }
        public DateTime? BlockedUntil { get; set; }  

        public int? UnblockedByUserId { get; set; }
        public User UnblockedBy { get; set; }
        public DateTime? UnblockedAt { get; set; }
        public string UnblockReason { get; set; }
    }
}
