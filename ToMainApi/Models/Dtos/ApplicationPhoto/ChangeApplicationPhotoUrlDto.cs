namespace ToMainApi.Models.Dtos.ApplicationPhoto
{
    public class ChangeApplicationPhotoUrlDto
    {
        public int ApplicationId { get; set; }
        public int PhotoId { get; set; }
        public string PhotoUrl { get; set; }
    }
}
