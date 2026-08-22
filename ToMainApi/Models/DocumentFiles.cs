using ToMainApi.Models.Enums;

namespace ToMainApi.Models
{
    public class DocumentFiles
    {
        public string Type { get; set; }
        public IFormFile Document { get; set; }
    }
}
