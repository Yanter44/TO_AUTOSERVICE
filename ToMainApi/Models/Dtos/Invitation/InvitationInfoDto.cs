namespace ToMainApi.Models.Dtos.Invitation
{
    public class InvitationInfoDto
    {
        public string Email { get; set; }
        public string RoleType { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
