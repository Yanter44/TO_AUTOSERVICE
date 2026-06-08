using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using ToMainApi.Common;
using ToMainApi.Models.Dtos.Agent;

namespace ToMainApi.Interfaces
{
    public interface IAgentService
    {
        Task<ServiceResponse<bool>> CreateNewApplication(CreateNewApplicationDto model);
    }
}
