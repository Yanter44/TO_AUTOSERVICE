using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Entities;

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
        public async Task<ServiceResponse<bool>> CreateNewApplication(CreateNewApplicationDto model)
        {
            var application = new Application
            {
                VehicleCategoryId = model.VehicleCategoryId,
                VIN = model.VIN,
                GosNumber = model.GosNumber,
                Brand = model.Brand,
                Model = model.Model,
                YearOfRelease = model.YearOfRelease,
                FIO = model.FIO,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                PtoId = model.PtoId,
                Documents = new List<ApplicationDocument>(),
                Photos = new List<ApplicationPhoto>(),
                Status = Models.Enums.ApplicationStatus.Moderated,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var document in model.DocumentFiles)
            {
                var url = await _cloudinaryService
                    .UploadFileAsync(document.Document);

                application.Documents.Add(new ApplicationDocument
                {
                    Type = document.Type,
                    Url = url
                });
            }

            foreach (var photo in model.VehiclePhotos)
            {
                var url = await _cloudinaryService
                    .UploadImageAsync(photo.Photo);

                application.Photos.Add(new ApplicationPhoto
                {
                    VehiclePhotoType = photo.VehiclePhotoType,
                    Url = url
                });
            }

            _dbcontext.Applications.Add(application);
            await _dbcontext.SaveChangesAsync();
            return new ServiceResponse<bool>
            {
                Success = true,
                Data = true
            };
        }

    }
}
