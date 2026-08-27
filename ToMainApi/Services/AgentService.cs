using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class AgentService : IAgentService
    {
        private readonly AppDbContext _dbcontext;
        private ICloudinaryService _cloudinaryService;
        public AgentService(AppDbContext dbcontext, ICloudinaryService cloudinaryService)
        {
            _dbcontext = dbcontext;
            _cloudinaryService = cloudinaryService;
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
        public async Task<ServiceResponse<List<AgentTransactionDto>>> GetMyBalanceTransactionStory(int userId)
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
