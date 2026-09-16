using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Interfaces
{
    public interface IUserBlockCache
    {
        Task<UserBlockInfoDto> GetBlockInfoAsync(int userId);
        Task InvalidateAsync(int userId);
    }
}
