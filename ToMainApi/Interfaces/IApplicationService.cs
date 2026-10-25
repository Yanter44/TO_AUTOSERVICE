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
        Task<ServiceResponse<ApplicationsMetricsDto>> GetApplicationsMetrics(UserContextDto userContext);
        Task<ServiceResponse<ApplicationDto>> CreateNewApplication(UserContextDto userContext, CreateNewApplicationDto model);
        Task<ServiceResponse<bool>> DeleteApplication(DeleteApplicationDto model);
        Task<ServiceResponse<bool>> AcceptApplication(AcceptApplicationDto model);
        Task<ServiceResponse<bool>> RejectApplication(RejectApplicationDto model);
    }
}
