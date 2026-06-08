using ToMainApi.Common;
using ToMainApi.Models.Dtos;

namespace ToMainApi.Interfaces
{
    public interface IPromptService
    {
        Task<ServiceResponse<List<PromtDtoRequest>>> GetAllPrompts(int UserId);
        Task<ServiceResponse<bool>> AddNewPromptAsync(int UserId, AddNewPromptDto model);
        Task<ServiceResponse<bool>> DeletePromptAsync(int UserId, DeletePromptDto model);
        Task<ServiceResponse<bool>> UpdatePromptAsync(int UserId, UpdatePromptDto model);
    }
}
