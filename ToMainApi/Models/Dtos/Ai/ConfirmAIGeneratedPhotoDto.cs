namespace ToMainApi.Models.Dtos.Ai
{
    public class ConfirmAIGeneratedPhotoDto
    {
        public int ApplicationId { get; set; }
        public int PhotoId { get; set; }
        public string ImageBase64 { get; set; }
    }
}
