namespace ToMainApi.Models.Entities
{
    public class DocumentUploadRequire
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public string DisplayName { get; set; }
        public bool IsRequire { get; set; }
    }
}
