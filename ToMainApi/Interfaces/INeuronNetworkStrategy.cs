using ToMainApi.Common;
using ToMainApi.Models.Dtos.Ai;
using ToMainApi.Models.Enums;

namespace ToMainApi.Interfaces
{
    public interface INeuronNetworkStrategy
    {
        SupportableAiProviders AiProvider { get;}

        Task<ServiceResponse<GeneratePhotoResponse>> ProcessPhotoAsync(Stream photo, List<string> prompts);
    }
}
