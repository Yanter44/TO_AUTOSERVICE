using ToMainApi.Common;
using ToMainApi.Models.Dtos.Auth;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;

namespace ToMainApi.Interfaces
{
    public interface IAuthService
    {
        Task<User> CheckUsers(LoginDto model);
        Task<ServiceResponse<string>> CreateAccessToken(UserDto userdtomodel);
        Task<ServiceResponse<int>> CheckRefreshToken(string token);
        Task<ServiceResponse<AccessAndRefreshTokenModel>> LoginUser(User usermodel);
        Task<ServiceResponse<string>> TryRegistration(TryRegistrationDto model);
        Task<ServiceResponse<bool>> ConfirmRegistrationCode(ConfirmCodeDto model);
        Task<ServiceResponse<AccessAndRefreshTokenModel>> FinishRegistration(RegistrationDto registermodel);
        Task<ServiceResponse<bool>> SignOut(string refreshtoken);
    }
}
