using MediatR;
using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Features.Events.Payments;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Application;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.Payments;
using ToMainApi.Models.Dtos.Pto;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _dbcontext;
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(AppDbContext dbcontext, 
            IMediator mediator,
            ILogger<PaymentService> logger)
        {
            _dbcontext = dbcontext;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ServiceResponse<bool>> Credit(CreditRequest request)
        {
            using var dbTransaction = await _dbcontext.Database.BeginTransactionAsync();
            try
            {
                var wallet = await _dbcontext.Users
                    .Where(x => x.Id == request.AgentId)
                    .Select(x => x.AgentProfile.Wallet)
                    .FirstOrDefaultAsync();

                if (wallet == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Кошелек агента не найден"
                    };
                }

                var existingTransaction = await _dbcontext.Transactions
                    .AnyAsync(x => x.IdempotencyKey == request.IdempotencyKey);

                if (existingTransaction)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Данная операция уже была выполнена"
                    };
                }

                _dbcontext.Transactions.Add(new Transaction
                {
                    ExternalId = Guid.NewGuid(),
                    IdempotencyKey = request.IdempotencyKey,
                    WalletId = wallet.Id,
                    Amount = request.Amount,
                    Description = request.Comment,
                    TransactionType = TransactionType.Credit.ToString(),
                    TransactionStatus = TransactionStatus.Completed.ToString(),
                    CreatedAt = DateTime.UtcNow
                });

                await _dbcontext.SaveChangesAsync();
                var balance = await _dbcontext.Transactions.Where(x => x.WalletId == wallet.Id).SumAsync(x =>
                                                            x.TransactionType == TransactionType.Credit.ToString()
                                                            ? x.Amount
                                                            : -x.Amount);

                var agentWallet = await _dbcontext.Wallets.Where(x => x.Id == wallet.Id).FirstOrDefaultAsync();
                agentWallet.Balance = balance;
                await _dbcontext.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                await _mediator.Publish(new CreditBalanceEvent(request.AgentId, request.Amount));

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Баланс успешно пополнен"
                };
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();

                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<bool>> Debit(DebitRequest request)
        {
            using var dbTransaction = await _dbcontext.Database.BeginTransactionAsync();

            try
            {
                var wallet = await _dbcontext.Users
                    .Where(x => x.Id == request.AgentId)
                    .Select(x => x.AgentProfile.Wallet)
                    .FirstOrDefaultAsync();

                if (wallet == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Кошелек агента не найден"
                    };
                }

                var existingTransaction = await _dbcontext.Transactions
                    .AnyAsync(x => x.IdempotencyKey == request.IdempotencyKey);

                if (existingTransaction)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Данная операция уже была выполнена"
                    };
                }

                var currentBalance = await _dbcontext.Transactions
                    .Where(x => x.WalletId == wallet.Id)
                    .SumAsync(x => x.TransactionType == TransactionType.Credit.ToString() ? x.Amount : -x.Amount);

                if (currentBalance < request.Amount)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Недостаточно средств"
                    };
                }

                _dbcontext.Transactions.Add(new Transaction
                {
                    ExternalId = Guid.NewGuid(),
                    IdempotencyKey = request.IdempotencyKey,
                    WalletId = wallet.Id,
                    Amount = request.Amount,
                    Description = request.Comment,
                    TransactionType = TransactionType.Debit.ToString(),
                    TransactionStatus = TransactionStatus.Completed.ToString(),
                    CreatedAt = DateTime.UtcNow
                });

                await _dbcontext.SaveChangesAsync();
                var balance = await _dbcontext.Transactions.Where(x => x.WalletId == wallet.Id).SumAsync(x =>
                                                       x.TransactionType == TransactionType.Credit.ToString()
                                                       ? x.Amount
                                                       : -x.Amount);

                var agentWallet = await _dbcontext.Wallets.Where(x => x.Id == wallet.Id).FirstOrDefaultAsync();
                agentWallet.Balance = balance;
                await _dbcontext.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                await _mediator.Publish(new DebitBalanceEvent(request.AgentId, request.Amount));
                return new ServiceResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Списание выполнено успешно"
                };
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();

                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ServiceResponse<List<TransactionDto>>> GetAllTransactions()
        {
            var transactions = await _dbcontext.Transactions
                .AsNoTracking()
                .Select(x => new TransactionDto
                {
                    ExternalId = x.ExternalId.ToString(),
                    AgentName = x.Wallet.Agent.User.FIO,
                    Amount = x.Amount,
                    TransactionType = x.TransactionType,
                    TransactionStatus = x.TransactionStatus,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return new ServiceResponse<List<TransactionDto>>
            {
                Data = transactions,
                Success = true
            };
        }
        public async Task<ServiceResponse<PagedResponse<TransactionDto>>> GetTransactions(TransactionFilterDto filter)
        {
            try
            {
                var query = _dbcontext.Transactions.AsNoTracking();

                if (filter.DateFrom.HasValue)
                    query = query.Where(x => x.CreatedAt >= filter.DateFrom.Value);

                if (filter.DateTo.HasValue)
                {
                    var endExclusive = filter.DateTo.Value.Date.AddDays(1);
                    query = query.Where(x => x.CreatedAt < endExclusive);
                }

                if (!string.IsNullOrWhiteSpace(filter.Status))
                    query = query.Where(x => x.TransactionStatus == filter.Status);

                if (!string.IsNullOrWhiteSpace(filter.Type))
                    query = query.Where(x => x.TransactionType == filter.Type);

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(x => new TransactionDto
                    {
                        ExternalId = x.ExternalId.ToString(),
                        AgentName = x.Wallet.Agent.User.FIO,
                        Amount = x.Amount,
                        TransactionType = x.TransactionType,
                        TransactionStatus = x.TransactionStatus,
                        Description = x.Description,
                        CreatedAt = x.CreatedAt
                    })
                    .ToListAsync();

                return new ServiceResponse<PagedResponse<TransactionDto>>
                {
                    Success = true,
                    Data = new PagedResponse<TransactionDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        Page = filter.Page,
                        PageSize = filter.PageSize,
                        TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении транзакций");
                return new ServiceResponse<PagedResponse<TransactionDto>>
                {
                    Data = null,
                    Success = false,
                    Message = ResponseMessages.UnSuccess
                };
            }
        }
    }
}
