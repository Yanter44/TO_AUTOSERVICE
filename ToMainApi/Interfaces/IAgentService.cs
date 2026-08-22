using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using ToMainApi.Common;
using ToMainApi.Models.Dtos.Agent;

namespace ToMainApi.Interfaces
{
    public interface IAgentService
    {
        Task<ServiceResponse<AgentDto>> GetMyProfile(int userId);
        Task<ServiceResponse<decimal>> GetMyBalance(int userId);
        Task<ServiceResponse<decimal>> GetMyDebtLimit(int userId);
        Task<ServiceResponse<List<AgentTransactionDto>>> GetMyBalanceTransactionStory(int userId);
    }
}
