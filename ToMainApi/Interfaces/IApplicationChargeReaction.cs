using ToMainApi.Common;

namespace ToMainApi.Interfaces
{
    public interface IApplicationChargeService
    {
        Task<ServiceResponse<bool>> ChargeForApplication(int applicationId, int agentUserId, CancellationToken ct = default);
    }
}
