namespace ToMainApi.Models.Dtos.User
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FIO { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public decimal? Balance { get; set; }
        public decimal? DebtLimit { get; set; }
        public DateTime RegDate { get; set; }
    }
}
