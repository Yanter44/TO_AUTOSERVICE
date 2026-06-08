namespace ToMainApi.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleType { get; set; }
        public AgentProfile AgentProfile { get; set; }
        public ModeratorProfile ModeratorProfile { get; set; }
        public AdminProfile AdminProfile { get; set; }
    }
}
