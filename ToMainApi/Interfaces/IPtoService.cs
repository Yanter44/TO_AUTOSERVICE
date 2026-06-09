using ToMainApi.Common;
using ToMainApi.Models.Dtos.Prompt;
using ToMainApi.Models.Dtos.Pto;

namespace ToMainApi.Interfaces
{
    public interface IPtoService
    {
        Task<ServiceResponse<List<PtoResponseDto>>> GetAllPtos();
        Task<ServiceResponse<bool>> AddNewPto(AddNewPtoDto model);
        Task<ServiceResponse<bool>> DeletePto(DeletePtoRequestDto model);
        Task<ServiceResponse<bool>> UpdatePto(UpdatePtoRequestDto model);
    }
}
