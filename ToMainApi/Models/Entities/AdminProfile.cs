namespace ToMainApi.Models.Entities
{
    public class AdminProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<Prompt> PromptsList { get; set; }
    }
}
