using ToMainApi.Common;
using ToMainApi.Models.Dtos.Ai;
using ToMainApi.Models.Dtos.NeuronNetwork;
using ToMainApi.Models.Enums;

namespace ToMainApi.Interfaces
{
    public interface INeuronNetwork
    {
        Task<ServiceResponse<GeneratePhotoResponse>> ProcessPhotoAsync(IReadOnlyList<Stream> photos, List<string> prompts, NeuronNetworkDto model);
    }
}
