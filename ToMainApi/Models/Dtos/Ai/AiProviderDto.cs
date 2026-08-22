using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Dtos.Ai
{
    public class AiProviderDto
    {
        public SupportableAiProviders Id { get; set; }
        public string Name { get; set; }
    }
}
