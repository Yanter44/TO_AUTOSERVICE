using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Prompt;
using ToMainApi.Models.Dtos.Pto;
using ToMainApi.Models.Entities;

namespace ToMainApi.Services
{
    public class PtoService : IPtoService
    {
        private readonly AppDbContext _dbcontext;
        public PtoService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<ServiceResponse<List<PtoResponseDto>>> GetAllPtos()
        {
            var ptos = await _dbcontext.Ptos
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
                    VehicleCategoryId = p.VehicleCategoryId,
                    Price = p.Price
                }).ToList()
            }).ToList();

            return new ServiceResponse<List<PtoResponseDto>>
            {
                Success = true,
                Data = result
            };
        }
        public async Task<ServiceResponse<bool>> AddNewPto(AddNewPtoDto model)
        {
            var pto = new Pto
            {
                Name = model.Name,
                RsaNumber = model.RsaNumber,
                Address = model.Address,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Login = model.Login,
                Password = model.Password,
                ApiKey = model.ApiKey,

                PricePolicies = model.PricePolicies.Select(x => new PtoPricePolicy
                {
                    VehicleCategoryId = x.VehicleCategoryId,
                    Price = x.Price
                }).ToList()
            };
            _dbcontext.Ptos.Add(pto);
            await _dbcontext.SaveChangesAsync();
            return new ServiceResponse<bool>() { Success = true };
        }
        public async Task<ServiceResponse<bool>> DeletePto(DeletePtoRequestDto model)
        {
            var existpto = await _dbcontext.Ptos.FirstOrDefaultAsync(x => x.Id == model.Id);
            if(existpto != null)
            {
                _dbcontext.Remove(existpto);
                await _dbcontext.SaveChangesAsync();
                return new ServiceResponse<bool>() { Success = true };
            }
            return new ServiceResponse<bool>() { Success = false };
        }
        public async Task<ServiceResponse<bool>> UpdatePto(UpdatePtoRequestDto model)
        {
            var pto = await _dbcontext.Ptos
                .Include(x => x.PricePolicies)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (pto == null)
            {
                return new ServiceResponse<bool> { Success = false };
            }
            pto.Name = model.Name;
            pto.RsaNumber = model.RsaNumber;
            pto.Address = model.Address;
            pto.Latitude = model.Latitude;
            pto.Longitude = model.Longitude;
            pto.Login = model.Login;
            pto.Password = model.Password;
            pto.ApiKey = model.ApiKey;
            _dbcontext.PtoPolicies.RemoveRange(pto.PricePolicies);
            pto.PricePolicies = model.PricePolicies.Select(x => new PtoPricePolicy
            {
                VehicleCategoryId = x.VehicleCategoryId,
                Price = x.Price
            }).ToList();
            await _dbcontext.SaveChangesAsync();
            return new ServiceResponse<bool> { Success = true };
        }
    }
}
