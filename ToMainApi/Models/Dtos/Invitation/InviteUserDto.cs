namespace ToMainApi.Models.Dtos.Invitation
{
    public class InviteUserDto
    {
        public string Email { get; set; }
        public string? Role { get; set; }
        public int? BranchId { get; set; }
    }
}
