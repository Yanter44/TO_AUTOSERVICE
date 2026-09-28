using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Refferal;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;

namespace ToMainApi.Services
{
    public class RefferalService : IRefferalService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<RefferalService> _logger;
        public RefferalService(AppDbContext dbContext, ILogger<RefferalService> logger)
        {
           _dbContext = dbContext;   
           _logger = logger;
        }

        public async Task<ServiceResponse<List<BranchWithMembersDto>>> GetMyBranches(UserContextDto userContextModel)
        {
            try
            {
                var existAgent = await _dbContext.AgentProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userContextModel.Id);
                if (existAgent == null)
                {
                    _logger.LogWarning("Пользователь {UserId} попытался получить ветки, но запись не найдена {UserId}", userContextModel.Id);
                    return new ServiceResponse<List<BranchWithMembersDto>> { Success = false, Message = "Произошла ошибка, агент не найден" };
                }

                var branches = await _dbContext.AgentBranches
                    .AsNoTracking()
                    .Where(b => b.OwnerAgentId == existAgent.Id)
                    .OrderBy(b => b.Name)
                    .Select(b => new
                    {
                        b.Id,
                        b.Name,
                        b.Fee,
                        b.IsActive,
                        b.CreatedAt,
                    })
                    .ToListAsync();

                if (branches.Count == 0)
                {
                    return new ServiceResponse<List<BranchWithMembersDto>>
                    {
                        Success = true,
                        Data = new List<BranchWithMembersDto>()
                    };
                }

                var branchIds = branches.Select(b => b.Id).ToList();

                var members = await _dbContext.AgentProfiles
                    .AsNoTracking()
                    .Where(a => a.BranchId != null && branchIds.Contains(a.BranchId.Value))
                    .Select(a => new
                    {
                        BranchId = a.BranchId!.Value,
                        Member = new BranchMemberDto
                        {
                            AgentId = a.Id,
                            Fio = a.User.FIO,
                            Email = a.User.Email,
                            RegDate = a.User.RegDate,
                            Balance = a.Wallet.Balance,
                            CurrentDebit = a.Wallet.CurrentDebt,
                            DebitLimit = a.Wallet.DebtLimit,
                        }
                    })
                    .ToListAsync();

                var membersByBranch = members
                    .GroupBy(m => m.BranchId)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Member).ToList());

                var result = branches.Select(b =>
                {
                    var branchMembers = membersByBranch.GetValueOrDefault(b.Id, new List<BranchMemberDto>());
                    return new BranchWithMembersDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Fee = b.Fee,
                        IsActive = b.IsActive,
                        CreatedAt = b.CreatedAt,
                        MemberCount = branchMembers.Count,
                        Members = branchMembers,
                    };
                }).ToList();

                _logger.LogInformation("Агент {AgentId} получил {Count} своих веток", existAgent.Id, result.Count);
                return new ServiceResponse<List<BranchWithMembersDto>>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Произошла ошибка при полученнии веток {Exception}", ex.Message);
                return new ServiceResponse<List<BranchWithMembersDto>> { Success = false, Message = "Произошла ошибка при получении веток" };
            }
        }
        public async Task<ServiceResponse<BranchDto>> CreateBranch(UserContextDto userContextModel, CreateBranchDto branchDtoModel)
        {
            try
            {
                var existAgent = await _dbContext.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == userContextModel.Id);
                if (existAgent == null)
                {
                    _logger.LogError($"Не удалось создать ветку поскольку не найден агент с id {userContextModel.Id}");
                    return new ServiceResponse<BranchDto>() { Success = false, Message = "Произошла ошибка, агент не найден" };
                }
                var branch = new AgentBranch()
                {
                    Name = branchDtoModel.Name,
                    Fee = branchDtoModel.Fee,
                    OwnerAgentId = existAgent.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.AgentBranches.Add(branch);
                await _dbContext.SaveChangesAsync();
                var branchmodel = new BranchDto()
                {
                    Name = branchDtoModel.Name,
                    Fee = branchDtoModel.Fee,
                    MemberCount = 0
                };
                return new ServiceResponse<BranchDto>() { Data = branchmodel, Success = true, Message = "Успешно создана ветка" };
            }
            catch (Exception ex)
            {
                _logger.LogError("Произошла ошибка при создании ветки {Exception}", ex);
                return new ServiceResponse<BranchDto>() { Data = null, Success = false, Message = "Произошла ошибка при создании ветки" };
            }

        }
        public async Task<decimal> CalculateTotalFee(string agentPath)
        {
            if (string.IsNullOrWhiteSpace(agentPath))
                return 0;

            var ids = agentPath
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            if (ids.Count == 0)
                return 0;

            var totalFee = await _dbContext.AgentProfiles
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .Where(x => x.BranchId != null && x.Branch.IsActive)
                .SumAsync(x => x.Branch.Fee);

            return totalFee;
        }
    }
}
