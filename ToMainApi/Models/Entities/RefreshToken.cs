namespace ToMainApi.Models.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
