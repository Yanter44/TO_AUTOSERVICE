using ToMainApi.Common;
using ToMainApi.Models.Dtos.Prompt;

namespace ToMainApi.Interfaces
{
    public interface IPromptService
    {
        Task<ServiceResponse<List<PromptDto>>> GetAllPrompts(int UserId);
        Task<ServiceResponse<PromptDto>> AddNewPromptAsync(int UserId, AddNewPromptDto model);
        Task<ServiceResponse<bool>> DeletePromptAsync(int UserId, int promptId);
        Task<ServiceResponse<PromptDto>> UpdatePromptAsync(int UserId, UpdatePromptDto model);
        Task<ServiceResponse<string>> GetPromptByUserId(int userId, int promptId);
        Task<ServiceResponse<List<string>>> GetPromptsByUserIdAndIds(int userId, List<int> promptsIds);
    }
}
