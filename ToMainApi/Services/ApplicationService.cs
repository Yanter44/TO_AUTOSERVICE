using MediatR;
using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Features.Application.Events;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Application;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ToMainApi.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly AppDbContext _dbcontext;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMediator _mediator;
        public ApplicationService(AppDbContext dbcontext, 
            ICloudinaryService cloudinaryService,
            IMediator mediator)
        {
            _dbcontext = dbcontext;
            _cloudinaryService = cloudinaryService;
            _mediator = mediator;
        }
        public async Task<ServiceResponse<PagedResponse<ApplicationDto>>> GetAgentApplications(int userId, PaginationDto pagination)
        {
            var agentId = await _dbcontext.AgentProfiles.Where(x => x.UserId == userId)
                                                        .Select(x => x.Id)
                                                        .FirstOrDefaultAsync();
            var query = _dbcontext.Applications
                .AsNoTracking()
                .Include(x => x.Photos)
                .Include(x => x.Documents)
                .Where(x => x.AgentId == agentId)
                .OrderByDescending(x => x.Id);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(application => new ApplicationDto
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
                    Status = application.Status.ToString(),

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
                })
                .ToListAsync();

            var result = new PagedResponse<ApplicationDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize)
            };

            return new ServiceResponse<PagedResponse<ApplicationDto>>
            {
                Data = result,
                Success = true
            };
        }
        public async Task<ServiceResponse<bool>> CreateNewApplication(int UserId, CreateNewApplicationDto model)
        {
            var agentId = await _dbcontext.AgentProfiles.Where(ap => ap.UserId == UserId)
                                                        .Select(ap => ap.Id)
                                                        .FirstOrDefaultAsync();
            if (agentId == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Профиль агента не найден в системе."
                };
            }
            var application = new Application
            {
                AgentId = agentId,
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
                Status = Models.Enums.ApplicationStatus.Moderated.ToString(),
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

            await _mediator.Publish(new ApplicationCreatedEvent(application.Id, UserId));

            return new ServiceResponse<bool>
            {
                Success = true,
                Data = true
            };
        }
        public async Task<ServiceResponse<PagedResponse<ApplicationDto>>> GetAllApplications(PaginationDto pagination)
        {
            var applications = _dbcontext.Applications
                .AsNoTracking()
                .Include(x => x.Photos)
                .Include(x => x.Documents);

            var totalCount = await applications.CountAsync();

            var listOfDtos = applications.Skip((pagination.Page - 1) * pagination.PageSize)
                                         .Take(pagination.PageSize)
            .Select(application => new ApplicationDto
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

            var result = new PagedResponse<ApplicationDto>
            {
                Items = listOfDtos,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize)
            };
            return new ServiceResponse<PagedResponse<ApplicationDto>>
            {
                Data = result,
                Success = true
            };
        }
        public async Task<ServiceResponse<bool>> DeleteApplication(DeleteApplicationDto model)
        {
            var existApplication = await _dbcontext.Applications.FirstOrDefaultAsync(x => x.Id == model.Id);
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
