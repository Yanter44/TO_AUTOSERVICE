using ToMainApi.Common;
using ToMainApi.Models.Dtos.Application;

namespace ToMainApi.Interfaces
{
    public interface IApplicationService
    {
        Task<ServiceResponse<List<ApplicationDto>>> GetAllApplications();
        Task<ServiceResponse<bool>> DeleteApplication(DeleteApplicationDto model);
    }
}
