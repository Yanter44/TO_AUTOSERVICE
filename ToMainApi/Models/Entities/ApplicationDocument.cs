using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Entities
{
    public class ApplicationDocument
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public DocumentType Type { get; set; }

        public string Url { get; set; }  
    }
}
