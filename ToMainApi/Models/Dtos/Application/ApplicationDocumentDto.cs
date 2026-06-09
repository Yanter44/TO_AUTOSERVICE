using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Dtos.Application
{
    public class ApplicationDocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DocumentType Type { get; set; }
        public string Url { get; set; }
    }
}
