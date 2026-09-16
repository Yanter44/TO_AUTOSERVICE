using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Requirementss;
using ToMainApi.Models.Entities;

namespace ToMainApi.Services
{
    public class PhotoUploadRequirementService : IPhotoUploadRequirementService
    {
        private readonly AppDbContext _dbContext;
        public PhotoUploadRequirementService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ServiceResponse<PhotoRequireDto>> EditPhotoRequirement(EditPhotoRequirementDto model)
        {
            var existRequirement = await _dbContext.PhotoUploadRequires.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
            if (existRequirement != null)
            {

                existRequirement.IsRequire = model.IsRequire;
                existRequirement.DisplayName = model.DisplayName;
                existRequirement.PhotoType = model.PhotoType;
                await _dbContext.SaveChangesAsync();
                var returnmodel = new PhotoRequireDto()
                {
                    Id = existRequirement.Id,
                    DisplayName = existRequirement.DisplayName,
                    PhotoType = model.PhotoType,
                    IsRequire = existRequirement.IsRequire,
                };
                return new ServiceResponse<PhotoRequireDto>()
                {
                    Data = returnmodel,
                    Success = true,
                    Message = "Успешно изменено требование"
                };
            }
            return new ServiceResponse<PhotoRequireDto>()
            {
                Success = false,
                Message = "Не удалось применить изменение к требованию"
            };
        }
        public async Task<ServiceResponse<List<PhotoRequireDto>>> GetAllPhotoRequirements()
        {
            var allrequirements = await _dbContext.PhotoUploadRequires.AsNoTracking().ToListAsync();
            var dtos = allrequirements.Select(x => new PhotoRequireDto
            {
                Id = x.Id,
                PhotoType = x.PhotoType, 
                DisplayName = x.DisplayName,
                IsRequire = x.IsRequire
            }).ToList();
            return new ServiceResponse<List<PhotoRequireDto>> () { Data = dtos, Success = true };
        }
        public async Task<ServiceResponse<PhotoRequireDto>> AddPhotoRequirement(AddPhotoRequirementDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.PhotoType) || string.IsNullOrWhiteSpace(model.DisplayName))
            {
                return new ServiceResponse<PhotoRequireDto>
                {
                    Success = false,
                    Message = "Заполните все обязательные поля (системный тип и отображаемое имя)."
                };
            }
            var isDuplicate = await _dbContext.PhotoUploadRequires.AnyAsync(x => x.PhotoType.ToLower() == model.PhotoType.ToLower());

            if (isDuplicate)
            {
                return new ServiceResponse<PhotoRequireDto>
                {
                    Success = false,
                    Message = $"Требование с системным типом '{model.PhotoType}' уже существует в базе данных."
                };
            }
            var newRequirement = new PhotoUploadRequire
            {
                PhotoType = model.PhotoType.Trim(),
                DisplayName = model.DisplayName.Trim(),
                IsRequire = model.IsRequire
            };

            await _dbContext.PhotoUploadRequires.AddAsync(newRequirement);
            await _dbContext.SaveChangesAsync();

            var resultDto = new PhotoRequireDto
            {
                Id = newRequirement.Id,
                PhotoType = newRequirement.PhotoType,
                DisplayName = newRequirement.DisplayName,
                IsRequire = newRequirement.IsRequire
            };

            return new ServiceResponse<PhotoRequireDto>
            {
                Data = resultDto,
                Success = true,
                Message = "Новое требование к фотографии успешно добавлено."
            };
        }
        public async Task<ServiceResponse<bool>> DeletePhotoRequirement(int photorequirementId)
        {
            var existrequirement = await _dbContext.PhotoUploadRequires.FirstOrDefaultAsync(x => x.Id == photorequirementId);
            if (existrequirement != null)
            {
                 _dbContext.Remove(existrequirement);
                await _dbContext.SaveChangesAsync();
                return new ServiceResponse<bool>() { Success = true };
            }
            return new ServiceResponse<bool>() { Success = false };
        }
    }
}
