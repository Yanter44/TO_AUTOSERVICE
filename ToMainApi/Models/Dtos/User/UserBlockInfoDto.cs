namespace ToMainApi.Models.Dtos.User
{
    public class UserBlockInfoDto
    {
        public bool IsBlocked { get; set; }
        public string? Reason { get; set; }
        public DateTime? BlockedUntil { get; set; }
    }
}
