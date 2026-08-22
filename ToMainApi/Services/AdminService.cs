using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Admin;
using ToMainApi.Models.Dtos.Agent;

namespace ToMainApi.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _dbcontext;
        public AdminService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<ServiceResponse<AdminDto>> GetMyProfile(int userId)
        {
            var existUser = await _dbcontext.Users
                .Include(x => x.AdminProfile)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (existUser != null)
            {
                var dto = new AdminDto
                {
                    Name = existUser.FIO,
                    Role = existUser.RoleType.ToString()
                };
                return new ServiceResponse<AdminDto>
                {
                    Data = dto,
                    Success = true
                };
            }
            return new ServiceResponse<AdminDto>() { Success = false };
        }
        public async Task<ServiceResponse<bool>> ChangeUserRole()
        {
            return null;
        }
        public async Task<ServiceResponse<bool>> ChangeUserDebtLimit(ChangeUserDebtLimitDto model)
        {
            var existuser = await _dbcontext.Users.Include(x => x.AgentProfile).FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (existuser != null)
            {
                existuser.AgentProfile.Wallet.DebtLimit = model.NewDebtLimit;
            }
            return null;
        }
    }
}
