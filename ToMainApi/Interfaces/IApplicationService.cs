using ToMainApi.Common;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Application;
using ToMainApi.Models.Dtos.Pagination;

namespace ToMainApi.Interfaces
{
    public interface IApplicationService
    {
        Task<ServiceResponse<PagedResponse<ApplicationDto>>> GetAllApplications(PaginationDto pagination);
        Task<ServiceResponse<PagedResponse<ApplicationDto>>> GetAgentApplications(int userId, PaginationDto pagination);
        Task<ServiceResponse<bool>> CreateNewApplication(int UserId, CreateNewApplicationDto model );
        Task<ServiceResponse<bool>> DeleteApplication(DeleteApplicationDto model);
    }
}
