using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbcontext;
        public UserService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<ServiceResponse<UserDto>> GetUserById(int userId)
        {
            var existUser = await _dbcontext.Users.AsNoTracking().Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (existUser != null)
            {
                return new ServiceResponse<UserDto>()
                {
                    Data = new UserDto()
                    {
                        UserId = userId,
                        FIO = existUser.FIO,
                        Email = existUser.Email,
                        Role = existUser.RoleType,
                        RegDate = existUser.RegDate,
                    },
                    Success = true
                };
            }
            return new ServiceResponse<UserDto>() { Success = false, Message = "Пользователь не найден :("};
        }

        public async Task<ServiceResponse<List<UserDto>>> GetAllUsers()
        {
            List<UserDto> users = new List<UserDto>();

            var agents = await _dbcontext.Users
                .AsNoTracking()
                .Where(x => x.RoleType == "Agent")
                .Include(x => x.AgentProfile)
                .ThenInclude(x => x.Wallet)
                .Select(x => new UserDto
                {
                    UserId = x.Id,
                    FIO = x.FIO,
                    Email = x.Email,
                    Role = x.RoleType,
                    Balance = x.AgentProfile.Wallet.Balance,
                    DebtLimit = x.AgentProfile.Wallet.DebtLimit,
                    RegDate = x.RegDate
                }).ToListAsync();

            var admins = await _dbcontext.Users
                .AsNoTracking()
                .Where(x => x.RoleType == "Admin")
                .Include(x => x.AdminProfile)
                .Select(x => new UserDto
                {
                    UserId = x.Id,
                    FIO = x.FIO,
                    Email = x.Email,
                    Role = x.RoleType,
                    RegDate = x.RegDate,
                }).ToListAsync();
           
            foreach (var agent in agents)
            {
                users.Add(agent);
            }
            foreach(var admin in admins)
            {
                users.Add(admin);
            }
            return new ServiceResponse<List<UserDto>>
            {
                Success = true,
                Data = users
            };
        }
        public async Task<ServiceResponse<List<UserDto>>> GetAllUsersExcept(int currentUserId)
        {
            List<UserDto> users = new List<UserDto>();

            var agents = await _dbcontext.Users
                .AsNoTracking()
                .Where(x => x.RoleType == "Agent" && x.Id != currentUserId)
                .Include(x => x.AgentProfile)
                .ThenInclude(x => x.Wallet)
                .Select(x => new UserDto
                {
                    UserId = x.Id,
                    FIO = x.FIO,
                    Email = x.Email,
                    Role = x.RoleType,
                    Balance = x.AgentProfile.Wallet.Balance,
                    DebtLimit = x.AgentProfile.Wallet.DebtLimit,
                    RegDate = x.RegDate
                })
                .ToListAsync();

            var admins = await _dbcontext.Users
                .AsNoTracking()
                .Where(x => x.RoleType == "Admin" && x.Id != currentUserId)
                .Include(x => x.AdminProfile)
                .Select(x => new UserDto
                {
                    UserId = x.Id,
                    FIO = x.FIO,
                    Email = x.Email,
                    Role = x.RoleType,
                    RegDate = x.RegDate
                })
                .ToListAsync();

            users.AddRange(agents);
            users.AddRange(admins);

            return new ServiceResponse<List<UserDto>>
            {
                Success = true,
                Data = users
            };
        }
        public async Task<ServiceResponse<List<AgentDto>>> GetAllAgents()
        {
            var result = await _dbcontext.Users
                .AsNoTracking()
                .Where(x => x.RoleType == "Agent")
                .Select(x => new AgentDto
                {
                    Id = x.Id,
                    Name = x.FIO,
                    Role = x.RoleType,
                    Balance = x.AgentProfile.Wallet.Balance
                })
                .ToListAsync();

       
            return new ServiceResponse<List<AgentDto>>
            {
                Data = result,
                Success = true
            };
        }
        public async Task<List<UserDto>> GetUsersByRoles(IEnumerable<string> roles)
        {
            return await _dbcontext.Users
                .AsNoTracking()
                .Where(u => roles.Contains(u.RoleType))
                .Select(u => new UserDto
                {
                    UserId = u.Id,
                    FIO = u.FIO,
                    Email = u.Email,
                    Role = u.RoleType,
                    RegDate = u.RegDate
                })
                .ToListAsync();
        }


    }
}
