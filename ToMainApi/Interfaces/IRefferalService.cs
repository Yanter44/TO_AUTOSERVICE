using ToMainApi.Common;
using ToMainApi.Models.Dtos.Refferal;
using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Interfaces
{
    public interface IRefferalService
    {
        Task<ServiceResponse<List<BranchWithMembersDto>>> GetMyBranches(UserContextDto userContextModel);
        Task<ServiceResponse<BranchDto>> CreateBranch(UserContextDto userContextModel, CreateBranchDto branchDtoModel);
        Task<decimal> CalculateTotalFee(string agentPath);
    }
}
