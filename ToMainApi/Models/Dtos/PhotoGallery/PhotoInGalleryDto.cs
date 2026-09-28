namespace ToMainApi.Models.Dtos.PhotoGallery
{
    public class PhotoInGalleryDto
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Group { get; set; }
        public string PublicId { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
