using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Ai;
using ToMainApi.Models.Dtos.ApplicationPhoto;

namespace ToMainApi.Services
{
    public class ApplicationPhotoService : IApplicationPhotoService
    {
        private readonly AppDbContext _dbcontext;
        private readonly ILogger<ApplicationPhotoService> _logger;
        private readonly ICloudinaryService _cloudinaryService;
        public ApplicationPhotoService(AppDbContext dbcontext, 
            ILogger<ApplicationPhotoService> logger, ICloudinaryService cloudinaryService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
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

        public async Task<ServiceResponse<bool>> ConfirmGeneratedPhoto(ConfirmAIGeneratedPhotoDto model)
        {
            var existPhotoData = await GetApplicationPhotoByApplicationIdAndPhotoId(model.ApplicationId, model.PhotoId);

            if (existPhotoData == null)
                return new ServiceResponse<bool> { Success = false, Message = "Фото не найдено" };

            var uploadnewPhotoUrlResult = await _cloudinaryService.UploadImageAsync(model.ImageBase64);
            if (!uploadnewPhotoUrlResult.Success || string.IsNullOrEmpty(uploadnewPhotoUrlResult.Data))
                return new ServiceResponse<bool> { Success = false, Message = uploadnewPhotoUrlResult.Message };

            var changeapplicationphotourlmodel = new ChangeApplicationPhotoUrlDto
            {
                ApplicationId = model.ApplicationId,
                PhotoId = model.PhotoId,
                PhotoUrl = uploadnewPhotoUrlResult.Data,
            };

            var changedPhotoUrlResult = await ChangeApplicationPhotoUrl(changeapplicationphotourlmodel);
            if (!changedPhotoUrlResult.Success)
            {
                await _cloudinaryService.DeleteImageByUrl(uploadnewPhotoUrlResult.Data);
                return new ServiceResponse<bool> { Success = false, Message = changedPhotoUrlResult.Message };
            }

            var deleteImageResult = await _cloudinaryService.DeleteImageByUrl(existPhotoData.Data.Url);
            if (!deleteImageResult.Success)
                _logger.LogWarning("Не удалось удалить старую картинку: {Url}", existPhotoData.Data.Url);

            return new ServiceResponse<bool> { Success = true, Data = true };
        }
         
        public async Task<ServiceResponse<ApplicationPhotoDto>> ChangeApplicationPhotoUrl(ChangeApplicationPhotoUrlDto model)
        {
            try
            {
                var existApplicationPhoto = await _dbcontext.ApplicationPhotos
                 .Where(x => x.Id == model.PhotoId)
                 .Where(x => x.ApplicationId == model.ApplicationId).FirstOrDefaultAsync();

                if (existApplicationPhoto != null)
                {
                    existApplicationPhoto.Url = model.PhotoUrl;
                    await _dbcontext.SaveChangesAsync();
                    return new ServiceResponse<ApplicationPhotoDto>()
                    {
                        Data = new ApplicationPhotoDto()
                        {
                            Id = existApplicationPhoto.Id,
                            Url = existApplicationPhoto.Url,
                            VehiclePhotoType = existApplicationPhoto.VehiclePhotoType
                        },
                        Success = true,
                        Message = "Ссылка на фотографию успешно изменена"
                    };
                }
                return new ServiceResponse<ApplicationPhotoDto>() { Data = null, Success = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при попытке изменить фотографию");
                return new ServiceResponse<ApplicationPhotoDto>() { Data = null, Success = false};
            }
        }
    }
}
