using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
