using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class AgentService : IAgentService
    {
        private readonly AppDbContext _dbcontext;
        private ICloudinaryService _cloudinaryService;
        private ILogger<AgentService> _logger;
        public AgentService(AppDbContext dbcontext, 
            ICloudinaryService cloudinaryService, 
            ILogger<AgentService> logger)
        {
            _dbcontext = dbcontext;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }
        public async Task<ServiceResponse<AgentDto>> GetMyProfile(int userId)
        {
            var existUser = await _dbcontext.Users
                .Include(x => x.AgentProfile)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (existUser != null)
            {
                var dto = new AgentDto
                {
                    FIO = existUser.FIO,
                    //Role = existUser.RoleType.ToString()
                };
                return new ServiceResponse<AgentDto>
                {
                    Data = dto,
                    Success = true
                };
            }
            return new ServiceResponse<AgentDto>() { Success = false };
        }
        public async Task<ServiceResponse<decimal>> GetMyBalance(int userId)
        {
            var walletId = await _dbcontext.Users
                .Where(x => x.Id == userId)
                .Select(x => x.AgentProfile.Wallet.Id)
                .FirstOrDefaultAsync();

            if (walletId == 0)
            {
                return new ServiceResponse<decimal>
                {
                    Success = false,
                    Message = "Кошелек не найден"
                };
            }

            var balance = await _dbcontext.Transactions
                .Where(x => x.WalletId == walletId)
                .SumAsync(x =>
                    x.TransactionType == TransactionType.Credit.ToString()
                        ? x.Amount
                        : -x.Amount);

            return new ServiceResponse<decimal>
            {
                Success = true,
                Data = balance
            };
        }
        public async Task<ServiceResponse<decimal>> GetMyDebtLimit(int userId)
        {
            var wallet = await _dbcontext.Wallets.FirstOrDefaultAsync(x => x.Agent.UserId == userId);
            if(wallet == null)
            {
                return new ServiceResponse<decimal>
                {
                    Success = false,
                    Message = "Кошелек не найден"
                };
            }
            return new ServiceResponse<decimal>
            {
                Data = wallet.DebtLimit,
                Success = true,
            };
        }
        public async Task<ServiceResponse<decimal>> GetMyCurrentDebt(int userId)
        {
            var wallet = await _dbcontext.Wallets.FirstOrDefaultAsync(x => x.Agent.UserId == userId);
            if(wallet == null)
            {
                return new ServiceResponse<decimal>
                {
                    Success = false,
                    Message = "Кошелек не найден"
                };
            }
            return new ServiceResponse<decimal>
            {
                Data = wallet.CurrentDebt,
                Success = true,
            };
        }
        public async Task<ServiceResponse<PagedResponse<AgentTransactionDto>>> GetMyBalanceTransactionStory(UserContextDto userContext, 
            PaginationDto paginationModel)
        {
            try
            {

                var query = _dbcontext.Transactions
                    .AsNoTracking()
                    .Where(t => t.Wallet.Agent.UserId == userContext.Id)
                    .Select(t => new AgentTransactionDto
                    {
                        Amount = t.Amount,
                        TransactionType = t.TransactionType,
                        TransactionStatus = t.TransactionStatus,
                        Description = t.Description,
                        CreatedAt = t.CreatedAt
                    });


                var totalCount = await query.CountAsync();

  
                var items = await query
                    .OrderByDescending(t => t.CreatedAt) 
                    .Skip((paginationModel.Page - 1) * paginationModel.PageSize)
                    .Take(paginationModel.PageSize)
                    .ToListAsync();

                return new ServiceResponse<PagedResponse<AgentTransactionDto>>
                {
                    Success = true,
                    Data = new PagedResponse<AgentTransactionDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        Page = paginationModel.Page,
                        PageSize = paginationModel.PageSize,
                        TotalPages = (int)Math.Ceiling((double)totalCount / paginationModel.PageSize)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Ошибка получения истории транзакций. UserId: {UserId}", userContext.Id);
                return new ServiceResponse<PagedResponse<AgentTransactionDto>>
                {
                    Success = false,
                    Message = "Ошибка получения истории транзакций",
                    Data = new PagedResponse<AgentTransactionDto>
                    {
                        Items = new List<AgentTransactionDto>(),
                        TotalCount = 0,
                        Page = paginationModel.Page,
                        PageSize = paginationModel.PageSize,
                        TotalPages = 0
                    }
                };
            }
        }
        public async Task<ServiceResponse<List<AgentTransactionDto>>> GetMyAllBalanceTransactionStory(int userId)
        {
            var wallet = await _dbcontext.Wallets.Include(x => x.Transactions).FirstOrDefaultAsync(x => x.Agent.UserId == userId);
            if (wallet == null)
            {
                return new ServiceResponse<List<AgentTransactionDto>>
                {
                    Success = false,
                    Message = "Кошелек не найден"
                };
            }

            var transactions = wallet.Transactions
                .Select(transaction => new AgentTransactionDto
                {
                    Amount = transaction.Amount,
                    TransactionType = transaction.TransactionType,
                    TransactionStatus = transaction.TransactionStatus,
                    Description = transaction.Description,
                    CreatedAt = transaction.CreatedAt
                })
                .ToList();

            return new ServiceResponse<List<AgentTransactionDto>>
            {
                Data = transactions,
                Success = true
            };
        }

    }
}
