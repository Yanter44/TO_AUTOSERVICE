using ToMainApi.Common;
using ToMainApi.Models.Dtos;

namespace ToMainApi.Interfaces
{
    public interface IPromptService
    {
        Task<ServiceResponse<bool>> AddNewPromptAsync(AddNewPromptDto model);
        Task<ServiceResponse<bool>> DeletePromptAsync(DeletePromptDto model);
        Task<ServiceResponse<bool>> UpdatePromptAsync(UpdatePromptDto model);
    }
}
