using ToMainApi.Common;
using ToMainApi.Models.Dtos;
using ToMainApi.Models.Entities;

namespace ToMainApi.Interfaces
{
    public interface IAuthService
    {
        Task<User> CheckUsers(LoginDto model);
        Task<ServiceResponse<string>> LoginUser(User usermodel);
        Task<ServiceResponse<string>> TryRegistration(TryRegistrationDto model);
        Task<ServiceResponse<bool>> ConfirmRegistrationCode(ConfirmCodeDto model);
        Task<ServiceResponse<string>> FinishRegistration(RegistrationDto registermodel);
    }
}
