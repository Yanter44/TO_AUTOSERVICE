using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using ToMainApi.Common;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Interfaces
{
    public interface IAgentService
    {
        Task<ServiceResponse<AgentDto>> GetMyProfile(int userId);
        Task<ServiceResponse<decimal>> GetMyBalance(int userId);
        Task<ServiceResponse<decimal>> GetMyDebtLimit(int userId);
        Task<ServiceResponse<decimal>> GetMyCurrentDebt(int userId);
        Task<ServiceResponse<List<AgentTransactionDto>>> GetMyAllBalanceTransactionStory(int userId);
        Task<ServiceResponse<PagedResponse<AgentTransactionDto>>> GetMyBalanceTransactionStory(UserContextDto userContext, PaginationDto paginationModel);
    }
}
