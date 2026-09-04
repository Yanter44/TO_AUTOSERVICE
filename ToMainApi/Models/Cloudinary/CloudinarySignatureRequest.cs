namespace ToMainApi.Models.Cloudinary
{
    public class CloudinarySignatureRequest
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
    }
}
