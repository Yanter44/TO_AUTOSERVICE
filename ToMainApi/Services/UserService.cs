using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;

namespace ToMainApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbcontext;
        public UserService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<ServiceResponse<AgentDto>> GetAgentByUserId(int userId)
        {
            var agent = await _dbcontext.AgentProfiles
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Wallet)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (agent == null)
            {
                return new ServiceResponse<AgentDto>
                {
                    Data = null,
                    Success = false,
                    Message = "Агент не найден"
                };
            }

            return new ServiceResponse<AgentDto>
            {
                Data = new AgentDto
                {
                    Id = agent.Id,
                    UserId = agent.UserId,
                    FIO = agent.User.FIO,
                    Email = agent.User.Email,
                    Balance = agent.Wallet?.Balance ?? 0,
                    DebtLimit = agent.Wallet?.DebtLimit ?? 0
                },
                Success = true,
                Message = "Агент найден"
            };
        }
        public async Task<ServiceResponse<List<UserDto>>> GetUsersByIds(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return new ServiceResponse<List<UserDto>>
                {
                    Data = new List<UserDto>(),
                    Success = true,
                    Message = "Список ID пуст"
                };
            }

            var users = await _dbcontext.Users
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new UserDto
                {
                    UserId = x.Id,
                    FIO = x.FIO,
                    Email = x.Email,
                    Role = x.RoleType,
                    RegDate = x.RegDate
                })
                .ToListAsync();

            return new ServiceResponse<List<UserDto>>
            {
                Data = users,
                Success = true,
                Message = $"Найдено {users.Count} пользователей"
            };
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
        public async Task<ServiceResponse<PagedResponse<UserDto>>> GetUsers(PaginationDto paginationModel)
        {
            var query = _dbcontext.Users
                .AsNoTracking()
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((paginationModel.Page - 1) * paginationModel.PageSize)
                .Take(paginationModel.PageSize)
                .ToListAsync();

            var result = users.Select(x =>
            {
                var userDto = new UserDto
                {
                    UserId = x.Id,
                    FIO = x.FIO,
                    Email = x.Email,
                    Role = x.RoleType,
                    RegDate = x.RegDate
                };

                if (x.RoleType == "Agent" && x.AgentProfile?.Wallet != null)
                {
                    userDto.Balance = x.AgentProfile.Wallet.Balance;
                    userDto.DebtLimit = x.AgentProfile.Wallet.DebtLimit;        
                }

                return userDto;
            }).ToList();

            return new ServiceResponse<PagedResponse<UserDto>>
            {
                Success = true,
                Data = new PagedResponse<UserDto>
                {
                    Items = result,
                    TotalCount = totalCount,
                    Page = paginationModel.Page,
                    PageSize = paginationModel.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / paginationModel.PageSize)
                }
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
                    FIO = x.FIO,
                    //Role = x.RoleType,
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
