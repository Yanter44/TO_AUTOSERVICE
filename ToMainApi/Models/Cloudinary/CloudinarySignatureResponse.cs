namespace ToMainApi.Models.Cloudinary
{
    public class CloudinarySignatureResponse
    {
        public string Signature { get; set; }
        public long Timestamp { get; set; }
        public string ApiKey { get; set; }
        public string CloudName { get; set; }
        public string Folder { get; set; }
        public string UploadPreset { get; set; }
        public long MaxFileSize { get; set; }
    }
}
