using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Dtos.Ai
{
    public class AiPhotoUploadRequest
    {
        public int ApplicationId { get; set; }
        public int PhotoId { get; set; }
        public SupportableAiProviders AiProvider { get; set; }
        public List<int> PromptsIds { get; set; }
    }
}
