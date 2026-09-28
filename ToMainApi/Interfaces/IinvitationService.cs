using ToMainApi.Common;
using ToMainApi.Models.Dtos.Invitation;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;

namespace ToMainApi.Interfaces
{
    public interface IinvitationService
    {
        Task<ServiceResponse<bool>> InviteUser(InviteUserDto model, UserContextDto userContextModel);
        Task<ServiceResponse<InvitationInfoDto>> GetInvitationByToken(string token);
        Task<ServiceResponse<UserInvitation>> Consume(string token);
    }
}
