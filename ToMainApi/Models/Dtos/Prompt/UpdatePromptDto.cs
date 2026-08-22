namespace ToMainApi.Models.Dtos.Prompt
{
    public class UpdatePromptDto
    {
        public int PromptId { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
    }
}
