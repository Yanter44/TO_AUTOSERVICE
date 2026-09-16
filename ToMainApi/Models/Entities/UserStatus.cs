namespace ToMainApi.Models.Entities
{
    public class UserStatus
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public bool IsBlocked { get; set; }
        public DateTime? BlockedUntil { get; set; }
    }
}
