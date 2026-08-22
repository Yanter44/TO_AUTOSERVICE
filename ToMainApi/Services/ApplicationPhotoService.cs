using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.ApplicationPhoto;

namespace ToMainApi.Services
{
    public class ApplicationPhotoService : IApplicationPhotoService
    {
        private readonly AppDbContext _dbcontext; 
        public ApplicationPhotoService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<ServiceResponse<List<ApplicationPhotoDto>>> GetAllApplicationPhotos()
        {
            var result = await _dbcontext.ApplicationPhotos.AsNoTracking().Select(x => new ApplicationPhotoDto()
            {
                Id = x.Id,
                VehiclePhotoType = x.VehiclePhotoType,
                Url = x.Url,
            }).ToListAsync();

            return new ServiceResponse<List<ApplicationPhotoDto>>()
            {
                Data = result,
                Success = true
            };
        }
        public async Task<ServiceResponse<List<ApplicationPhotoDto>>> GetApplicationPhotosByApplicationId(int applicationId)
        {
            var result = await _dbcontext.ApplicationPhotos
                .AsNoTracking()
                .Where(x => x.ApplicationId == applicationId)
                .Select(x => new ApplicationPhotoDto()
                {
                    Id = x.Id,
                    VehiclePhotoType = x.VehiclePhotoType,
                    Url = x.Url
                })
                .ToListAsync();

            return new ServiceResponse<List<ApplicationPhotoDto>>()
            {
                Data = result,
                Success = true
            };
        }

        public async Task<ServiceResponse<ApplicationPhotoDto>> GetApplicationPhotoByApplicationIdAndPhotoId(int applicationId, int photoId)
        {
            var result = await _dbcontext.ApplicationPhotos
                .AsNoTracking()
                .Where(x => x.Id == photoId)
                .Where(x => x.ApplicationId == applicationId)
                .Select(x => new ApplicationPhotoDto()
                {
                    Id = x.Id,
                    VehiclePhotoType = x.VehiclePhotoType,
                    Url = x.Url
                }).FirstOrDefaultAsync();
            return new ServiceResponse<ApplicationPhotoDto>()
            {
                Data = result,
                Success = true
            };
        }
    }
}
