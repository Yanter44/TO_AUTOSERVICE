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
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext dbcontext, ILogger<UserService> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
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
                .AsQueryable()
                .Include(x => x.AgentProfile)
                .ThenInclude(x => x.Wallet);

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
        public async Task<ServiceResponse<bool>> BlockUser(UserContextDto userContext, BlockUserDto model)
        {
            try
            {
                if (userContext?.Id == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Не удалось определить текущего пользователя" };

                if (model.BlockUserId == userContext.Id.Value)
                    return new ServiceResponse<bool> { Success = false, Message = "Нельзя заблокировать самого себя" };

                var user = await _dbcontext.Users
                    .Include(u => u.Status)
                    .FirstOrDefaultAsync(u => u.Id == model.BlockUserId);

                if (user == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Пользователь не найден" };

                if (user.Status == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Профиль статуса не найден" };

                if (user.Status.IsBlocked)
                    return new ServiceResponse<bool> { Success = false, Message = "Пользователь уже заблокирован" };

                var now = DateTime.UtcNow;

                var block = new UserBlocks
                {
                    UserId = model.BlockUserId,
                    BlockedByUserId = userContext.Id.Value,
                    Reason = model.Reason,
                    BlockedAt = now,
                    BlockedUntil = model.BlockUntil,

                    UnblockedByUserId = null,
                    UnblockedAt = null,
                    UnblockReason = null
                };

                _dbcontext.UserBlocks.Add(block);
                user.Status.IsBlocked = true;
                user.Status.BlockedUntil = model.BlockUntil;

                await _dbcontext.SaveChangesAsync();

                return new ServiceResponse<bool> { Success = true, Data = true, Message = "Пользователь заблокирован" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка блокировки пользователя {UserId}", model.BlockUserId);
                return new ServiceResponse<bool> { Success = false, Message = "Внутренняя ошибка" };
            }
        }
        public async Task<ServiceResponse<bool>> UnblockUser(UserContextDto userContext, UnblockUserDto model)
        {
            try
            {
                if (userContext?.Id == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Не удалось определить текущего пользователя" };

                var user = await _dbcontext.Users
                    .Include(u => u.Status)
                    .FirstOrDefaultAsync(u => u.Id == model.UnblockUserId);

                if (user == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Пользователь не найден" };

                if (user.Status == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Профиль статуса не найден" };

                if (!user.Status.IsBlocked)
                    return new ServiceResponse<bool> { Success = false, Message = "Пользователь не заблокирован" };

                var activeBlock = await _dbcontext.UserBlocks
                    .Where(b => b.UserId == model.UnblockUserId && b.UnblockedAt == null)
                    .OrderByDescending(b => b.BlockedAt)
                    .FirstOrDefaultAsync();

                if (activeBlock == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Активная блокировка не найдена" };

                var now = DateTime.UtcNow;

                activeBlock.UnblockedByUserId = userContext.Id.Value;
                activeBlock.UnblockedAt = now;
                activeBlock.UnblockReason = model.UnblockReason;

                user.Status.IsBlocked = false;
                user.Status.BlockedUntil = null;

                await _dbcontext.SaveChangesAsync();

                return new ServiceResponse<bool> { Success = true, Data = true, Message = "Пользователь разблокирован" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка разблокировки пользователя {UserId}", model.UnblockUserId);
                return new ServiceResponse<bool> { Success = false, Message = "Внутренняя ошибка" };
            }
        }
        public async Task<ServiceResponse<bool>> ChangeUserDebtLimit(ChangeUserDebtLimitDto model)
        {
            var wallet = await _dbcontext.Wallets
                .FirstOrDefaultAsync(w => w.Agent.UserId == model.UserId);

            if (wallet == null)
                return new ServiceResponse<bool> { Success = false, Message = "Кошелёк не найден" };

            if (model.DebtLimit < 0)
                return new ServiceResponse<bool> { Success = false, Message = "Лимит не может быть отрицательным" };

            if (wallet.CurrentDebt > model.DebtLimit)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Новый лимит меньше текущего долга ({wallet.CurrentDebt} ₽)"
                };

            wallet.DebtLimit = model.DebtLimit;
            await _dbcontext.SaveChangesAsync();

            return new ServiceResponse<bool> { Success = true, Data = true };
        }
        public async Task<ServiceResponse<List<UserBlockHistoryDto>>> GetUserBlockHistory(int userId)
        {
            try
            {
                var userExists = await _dbcontext.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == userId);

                if (!userExists)
                    return new ServiceResponse<List<UserBlockHistoryDto>>
                    {
                        Success = false,
                        Message = "Пользователь не найден"
                    };

                var now = DateTime.UtcNow;

                var history = await _dbcontext.UserBlocks
                    .AsNoTracking()
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.BlockedAt)
                    .Select(b => new UserBlockHistoryDto
                    {
                        Id = b.Id,

                        BlockedAt = b.BlockedAt,
                        Reason = b.Reason,
                        BlockedByUserId = b.BlockedByUserId,
                        BlockedByFIO = b.BlockedBy.FIO,
                        BlockedUntil = b.BlockedUntil,

                        UnblockedAt = b.UnblockedAt,
                        UnblockReason = b.UnblockReason,
                        UnblockedByUserId = b.UnblockedByUserId,
                        UnblockedByFIO = b.UnblockedBy != null ? b.UnblockedBy.FIO : null,

                        IsActive = b.UnblockedAt == null
                                && (b.BlockedUntil == null || b.BlockedUntil > now),

                        IsPermanent = b.BlockedUntil == null,

                        WasAutoUnblocked = b.UnblockedAt != null && b.UnblockedByUserId == null
                    })
                    .ToListAsync();

                return new ServiceResponse<List<UserBlockHistoryDto>>
                {
                    Success = true,
                    Data = history,
                    Message = $"Найдено записей: {history.Count}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения истории блокировок пользователя {UserId}", userId);
                return new ServiceResponse<List<UserBlockHistoryDto>>
                {
                    Success = false,
                    Message = "Внутренняя ошибка"
                };
            }
        }

    }
}
