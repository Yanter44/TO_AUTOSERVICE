using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Application;

namespace ToMainApi.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly AppDbContext _dbcontext;
        public ApplicationService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<ServiceResponse<List<ApplicationDto>>> GetAllApplications()
        {
            var applications = await _dbcontext.Applications
                .Include(x => x.Photos)
                .Include(x => x.Documents)
                .ToListAsync();

            var listOfDtos = applications.Select(application => new ApplicationDto
            {
                Id = application.Id,

                VehicleCategoryId = application.VehicleCategoryId,
                VIN = application.VIN,
                GosNumber = application.GosNumber,
                Brand = application.Brand,
                Model = application.Model,
                YearOfRelease = application.YearOfRelease,

                FIO = application.FIO,
                Email = application.Email,
                PhoneNumber = application.PhoneNumber,

                PtoId = application.PtoId,
                CreatedAt = application.CreatedAt,
                Status = application.Status,

                Photos = application.Photos.Select(p => new ApplicationPhotoDto
                {
                    Id = p.Id,
                    FileName = $"photo_{p.Id}.jpg", 
                    VehiclePhotoType = p.VehiclePhotoType,
                    Url = p.Url
                }).ToList(),

                Documents = application.Documents.Select(d => new ApplicationDocumentDto
                {
                    Id = d.Id,
                    FileName = $"doc_{d.Id}.pdf", 
                    Type = d.Type,
                    Url = d.Url
                }).ToList()
            }).ToList();

            return new ServiceResponse<List<ApplicationDto>>
            {
                Data = listOfDtos,
                Success = true
            };
        }
        public async Task<ServiceResponse<bool>> DeleteApplication(DeleteApplicationDto model)
        {
            var existApplication = await _dbcontext.Applications
                .FirstOrDefaultAsync(x => x.Id == model.Id);
            if (existApplication == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false
                };
            }
            _dbcontext.Applications.Remove(existApplication);
            await _dbcontext.SaveChangesAsync();
            return new ServiceResponse<bool>
            {
                Success = true
            };
        }

    }
}
