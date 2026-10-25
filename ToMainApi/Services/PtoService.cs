using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.Prompt;
using ToMainApi.Models.Dtos.Pto;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class PtoService : IPtoService
    {
        private readonly AppDbContext _dbcontext;
        private readonly IInfisicalService _infisicalService;
        private readonly IArchiveCryptoService _archiveCrypto;
        public PtoService(
         AppDbContext dbcontext,
         IInfisicalService infisicalService,
         IArchiveCryptoService archiveCrypto)
        {
            _dbcontext = dbcontext;
            _infisicalService = infisicalService;
            _archiveCrypto = archiveCrypto;
        }
        public async Task<ServiceResponse<List<PtoResponseDto>>> GetAllPtos()
        {
            var ptos = await _dbcontext.Ptos
                .AsNoTracking()
                .Include(x => x.PricePolicies)
                .ToListAsync();

            var result = ptos.Select(x => new PtoResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                RsaNumber = x.RsaNumber,
                Address = x.Address,
                Latitude = x.Latitude,
                Longitude = x.Longitude,

                PricePolicies = x.PricePolicies.Select(p => new PtoPricePolicyDto
                {
                    VehicleCategoriesBySrtsId = p.VehicleCategoryBySrtsId,
                    Price = p.Price
                }).ToList()
            }).ToList();

            return new ServiceResponse<List<PtoResponseDto>>
            {
                Success = true,
                Data = result
            };
        }
        public async Task<ServiceResponse<PagedResponse<PtoResponseDto>>> GetPtos(UserContextDto userContext, PaginationDto paginationModel)
        {
            var query = _dbcontext.Ptos
                .AsNoTracking()
                .Include(x => x.PricePolicies)
                .Include(x => x.Experts)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var ptos = await query
                .Skip((paginationModel.Page - 1) * paginationModel.PageSize)
                .Take(paginationModel.PageSize)
                .ToListAsync();

            var result = ptos.Select(x => new PtoResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                RsaNumber = x.RsaNumber,
                Address = x.Address,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                PtoExperts = x.Experts.Select(e => new PtoExpertDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    MiddleName = e.MiddleName,
                    LastName = e.LastName,
                    SequenceNumber = e.SequenceNumber,
                }).ToList(),
                PricePolicies = x.PricePolicies.Select(p => new PtoPricePolicyDto
                {
                    VehicleCategoriesBySrtsId = p.VehicleCategoryBySrtsId,
                    Price = p.Price
                }).ToList()
            }).ToList();

            return new ServiceResponse<PagedResponse<PtoResponseDto>>
            {
                Success = true,
                Data = new PagedResponse<PtoResponseDto>
                {
                    Items = result,
                    TotalCount = totalCount,
                    Page = paginationModel.Page,
                    PageSize = paginationModel.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / paginationModel.PageSize)
                }
            };
        }

        public async Task<ServiceResponse<bool>> AddNewPto(AddNewPtoDto model)
        {
            var masterKeyBase64 = await _infisicalService.GetSecretAsync("CRYPTO_MASTER_KEY");
            var masterKey = Convert.FromBase64String(masterKeyBase64);
            try
            {
                var operatorEncrypted = await _archiveCrypto.EncryptAsync(model.OperatorKeyArchive, masterKey);
                var expertEncryptedList = new List<byte[]>();
                foreach (var expert in model.Experts)
                {
                    if (expert.KeyArchive is null || expert.KeyArchive.Length == 0)
                        return new ServiceResponse<bool> { Success = false, Message = $"У эксперта {expert.LastName} нет ключа" };

                    expertEncryptedList.Add(await _archiveCrypto.EncryptAsync(expert.KeyArchive, masterKey));
                }

                var pto = new Pto
                {
                    Name = model.Name,
                    RsaNumber = model.RsaNumber,
                    Address = model.Address,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,

                    OperatorCryptoKey = new PtoOperatorCryptoKey
                    {
                        EncryptedData = operatorEncrypted,
                        CreatedAt = DateTime.UtcNow,
                    },

                    Experts = model.Experts.Select((dto, i) => new PtoExpert
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        MiddleName = dto.MiddleName,
                        SequenceNumber = int.Parse(dto.SequenceNumber),
                        CryptoKey = new PtoExpertCryptoKey
                        {
                            EncryptedData = expertEncryptedList[i],
                            CreatedAt = DateTime.UtcNow,
                        },
                    }).ToList(),

                    PricePolicies = model.PricePolicies.Select(x => new PtoPricePolicy
                    {
                        VehicleCategoryBySrtsId = x.VehicleCategoriesBySrtsId,
                        Price = x.Price,
                    }).ToList(),
                };

                _dbcontext.Ptos.Add(pto);
                await _dbcontext.SaveChangesAsync();

                return new ServiceResponse<bool> { Success = true };
            }
            finally
            {
                CryptographicOperations.ZeroMemory(masterKey);
            }
        }
        
        public async Task<ServiceResponse<bool>> DeletePtoAsync(int ptoId)
        {
            var pto = await _dbcontext.Ptos.FindAsync(ptoId);
            if (pto == null || pto.IsDeleted)
                return new ServiceResponse<bool> { Success = false, Message = "ПТО не найдено" };

            pto.IsDeleted = true;
            pto.DeletedAt = DateTime.UtcNow;
            pto.IsActive = false;

            var affectedStatuses = new[]
            {
                ApplicationStatus.Validating.ToString(),
                ApplicationStatus.OnModeration.ToString(),
                ApplicationStatus.Approved.ToString(),
            };

            await _dbcontext.Applications
                .Where(a => a.PtoId == ptoId && affectedStatuses.Contains(a.Status))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.Status, ApplicationStatus.Error.ToString()));

            await _dbcontext.SaveChangesAsync();
            return new ServiceResponse<bool> { Data = true, Success = true, Message = ResponseMessages.Success };
        }
        public async Task<ServiceResponse<bool>> UpdatePto(UpdatePtoRequestDto model)
        {
            var pto = await _dbcontext.Ptos
                .Include(x => x.PricePolicies)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (pto == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "ПТО не найдено" };
            }
            pto.Name = model.Name;
            pto.RsaNumber = model.RsaNumber;
            pto.Address = model.Address;
            pto.Latitude = model.Latitude;
            pto.Longitude = model.Longitude;
            _dbcontext.PtoPolicies.RemoveRange(pto.PricePolicies);
            pto.PricePolicies = model.PricePolicies.Select(x => new PtoPricePolicy
            {
                VehicleCategoryBySrtsId = x.VehicleCategoriesBySrtsId,
                Price = x.Price
            }).ToList();
            await _dbcontext.SaveChangesAsync();
            return new ServiceResponse<bool> { Success = true };
        }
    }
}
