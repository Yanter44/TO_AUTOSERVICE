namespace ToMainApi.Models.Dtos.User
{
    public class BlockUserDto
    {
        public int BlockUserId { get; set; }
        public string Reason { get; set; }
        public DateTime? BlockUntil { get; set; }
    }
}
