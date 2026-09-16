using ToMainApi.Common;
using ToMainApi.Models.Dtos.NeuronNetwork;
using ToMainApi.Models.Dtos.Pagination;

namespace ToMainApi.Interfaces
{
    public interface INeuronNetworkService
    {
        Task<ServiceResponse<NeuronNetworkDto>> GetNeuronNetworkById(int neuronNetworkId);
        Task<ServiceResponse<List<NeuronNetworkDto>>> GetAllNeuronNetworks();
        Task<ServiceResponse<PagedResponse<NeuronNetworkDto>>> GetNeuronNetworks(PaginationDto paginationModel);
        Task<ServiceResponse<NeuronNetworkDto>> AddNewNeuronNetwork(AddNewNeuronNetworkDto model);
        Task<ServiceResponse<NeuronNetworkDto>> UpdateNeuronNetwork(UpdateNeuronNetworkDto model);
        Task<ServiceResponse<bool>> DeleteNeuronNetwork(int NeuronNetworkId);
    }
}
