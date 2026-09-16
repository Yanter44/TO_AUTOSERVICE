using ToMainApi.Common;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<AgentDto>> GetAgentByUserId(int userId);
        Task<ServiceResponse<UserDto>> GetUserById(int userId);
        Task<ServiceResponse<List<UserDto>>> GetUsersByIds(List<int> ids);
        Task<ServiceResponse<List<UserDto>>> GetAllUsers();
        Task<ServiceResponse<List<UserDto>>> GetAllUsersExcept(int currentUserId);
        Task<ServiceResponse<PagedResponse<UserDto>>> GetUsers(PaginationDto paginationModel);
        Task <ServiceResponse<List<AgentDto>>> GetAllAgents();
        Task<List<UserDto>> GetUsersByRoles(IEnumerable<string> roles);
        Task<ServiceResponse<List<UserBlockHistoryDto>>> GetUserBlockHistory(int userId);
        Task<ServiceResponse<bool>> BlockUser(UserContextDto userContext, BlockUserDto model);
        Task<ServiceResponse<bool>> UnblockUser(UserContextDto userContext, UnblockUserDto model);
        Task<ServiceResponse<bool>> ChangeUserDebtLimit(ChangeUserDebtLimitDto model);
    }
}
