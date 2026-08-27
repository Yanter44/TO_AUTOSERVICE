using ToMainApi.Common;
using ToMainApi.Models.Dtos.Admin;

namespace ToMainApi.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResponse<AdminDto>> GetMyProfile(int userId);
        Task<ServiceResponse<bool>> ChangeUserRole(ChangeUserRoleDto model);
    }
}
