using ToMainApi.Common;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Application;
using ToMainApi.Models.Dtos.Metrics;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Interfaces
{
    public interface IApplicationService
    {
        Task<ServiceResponse<PagedResponse<ApplicationDto>>> GetApplications(UserContextDto userContext, PaginationDto pagination);
        Task<ServiceResponse<ApplicationsMetricsDto>> GetApplicationsMetrics(UserContextDto usercontextmodel);
        Task<ServiceResponse<bool>> CreateNewApplication(int UserId, CreateNewApplicationDto model );
        Task<ServiceResponse<bool>> DeleteApplication(DeleteApplicationDto model);
    }
}
