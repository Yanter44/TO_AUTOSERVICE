namespace ToMainApi.Models.Entities
{
    public class PhotoUploadRequire
    {
        public int Id { get; set; }
        public string PhotoType { get; set; }
        public string DisplayName { get; set; }
        public bool IsRequire { get; set; }
    }
}
