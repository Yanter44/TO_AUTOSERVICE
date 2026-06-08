namespace ToMainApi.Models.Entities
{
    public class Prompt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
    }
}
