using MediatR;
using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Features.Events.Payments;
using ToMainApi.Interfaces;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class ApplicationChargeService : IApplicationChargeService
    {
        private readonly AppDbContext _dbcontext;
        private readonly IMediator _mediator;
        private readonly ILogger<ApplicationChargeService> _logger;
        private readonly IUserService _userService;
        public ApplicationChargeService(AppDbContext dbcontext, IMediator mediator, 
            ILogger<ApplicationChargeService> logger,
            IUserService userService)
        {
            _dbcontext = dbcontext;
            _mediator = mediator;
            _logger = logger;
            _userService = userService;
        }
        public async Task<ServiceResponse<bool>> ChargeForApplication(
       int applicationId, int userId, CancellationToken ct = default)
        {
            var application = await _dbcontext.Applications
                .Include(a => a.Agent).ThenInclude(ag => ag.Wallet)
                .FirstOrDefaultAsync(a => a.Id == applicationId, ct);

            if (application?.Agent?.Wallet == null)
            {
                _logger.LogError("Кошелёк не найден. ApplicationId: {Id}, UserId: {UserId}",
                    applicationId, userId);
                return new ServiceResponse<bool> { Success = false, Message = "Кошелёк агента не найден" };
            }

            var wallet = application.Agent.Wallet;

            var price = await GetPtoPriceForApplication(application.PtoId, application.VehicleCategoryId);
            decimal fromBalance, fromCredit;

            if (wallet.Balance >= price)
            {
                fromBalance = price;
                fromCredit = 0;
            }
            else
            {
                fromBalance = wallet.Balance;
                fromCredit = price - wallet.Balance;
                var newDebt = wallet.CurrentDebt + fromCredit;

                if (newDebt > wallet.DebtLimit)
                {
                    _logger.LogWarning(
                        "Превышен кредитный лимит. ApplicationId: {Id}, Debt: {Debt}, Limit: {Limit}",
                        applicationId, newDebt, wallet.DebtLimit);

                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = $"Недостаточно средств. Кредитный лимит превышен: долг {newDebt} ₽, лимит {wallet.DebtLimit} ₽"
                    };
                }
            }

            wallet.Balance -= fromBalance;
            wallet.CurrentDebt += fromCredit;

            _dbcontext.Transactions.Add(new Transaction
            {
                ExternalId = Guid.NewGuid(),
                IdempotencyKey = Guid.NewGuid(),
                WalletId = wallet.Id,
                Amount = price,
                TransactionType = TransactionType.Debit.ToString(),
                Description = $"Списание за заявку #{applicationId}",
                TransactionStatus = TransactionStatus.Completed.ToString(),
                CreatedAt = DateTime.UtcNow
            });

            await _dbcontext.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Списано {Price} ₽ (с баланса: {FromBalance}, в долг: {FromCredit}) за заявку {Id} у агента {UserId}",
                price, fromBalance, fromCredit, applicationId, userId);

            await _mediator.Publish(new DebitBalanceEvent(userId, price), ct);

            return new ServiceResponse<bool> { Success = true, Data = true };
        }
        private async Task<decimal> GetPtoPriceForApplication(int ptoId, int vehicleCategoryId)
        {
            var price = await _dbcontext.PtoPolicies.Where(x => x.PtoId == ptoId && x.VehicleCategoryId == vehicleCategoryId)
                                                    .Select(x => x.Price).FirstOrDefaultAsync();
            return price;
        }
    }
}
