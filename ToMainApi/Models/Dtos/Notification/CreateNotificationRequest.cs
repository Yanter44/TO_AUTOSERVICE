namespace ToMainApi.Models.Dtos.Notification
{
    public class CreateNotificationRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
    }
}
