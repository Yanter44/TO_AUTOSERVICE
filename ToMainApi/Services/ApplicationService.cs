using CloudinaryDotNet.Actions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Features.Application.Events;
using ToMainApi.Features.Application.Events.Applications;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Application;
using ToMainApi.Models.Dtos.Metrics;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ToMainApi.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly AppDbContext _dbcontext;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMediator _mediator;
        private readonly ILogger<ApplicationService> _logger;
        public ApplicationService(AppDbContext dbcontext, 
            ICloudinaryService cloudinaryService,
            IMediator mediator,
            ILogger<ApplicationService> logger)
        {
            _dbcontext = dbcontext;
            _cloudinaryService = cloudinaryService;
            _mediator = mediator;
            _logger = logger;
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
        public async Task<ServiceResponse<ApplicationsMetricsDto>> GetApplicationsMetrics(UserContextDto usercontextmodel)
        {
            var query = _dbcontext.Applications.AsQueryable();
            var today = DateTime.UtcNow.Date;

            if (!Enum.TryParse<Models.Enums.Role>(usercontextmodel.Role, true, out var role))
            {
                _logger.LogWarning($"Неизвестная роль: {usercontextmodel.Role}");
                return new ServiceResponse<ApplicationsMetricsDto>
                {
                    Data = null,
                    Success = false,
                    Message = "Неизвестная роль пользователя"
                };
            }
            try
            {
                IQueryable<Application> filteredQuery = role switch
                {
                    Models.Enums.Role.Admin => query,
                    Models.Enums.Role.Agent => query.Where(x => x.Agent != null && x.Agent.UserId == usercontextmodel.Id),
                    _ => null
                };

                if (filteredQuery == null)
                {
                    return new ServiceResponse<ApplicationsMetricsDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "У вас нет прав для просмотра этой информации"
                    };
                }
                var metrics = await filteredQuery
                    .GroupBy(x => 1)
                    .Select(g => new
                    {
                        Total = g.Count(),
                        InModeration = g.Count(x => x.Status == ApplicationStatus.Moderated.ToString()),
                        Approved = g.Count(x => x.Status == ApplicationStatus.Approved.ToString()),
                        Today = g.Count(x => x.CreatedAt.Date == today)
                    })
                    .FirstOrDefaultAsync();

                return new ServiceResponse<ApplicationsMetricsDto>
                {
                    Data = new ApplicationsMetricsDto
                    {
                        TotalApplicationsCount = metrics?.Total ?? 0,
                        TotalApplicationsInModerationCount = metrics?.InModeration ?? 0,
                        TotalApplicationsApprovedCount = metrics?.Approved ?? 0,
                        TotalApplicationsTodayCount = metrics?.Today ?? 0
                    },
                    Success = true,
                    Message = ResponseMessages.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении метрик для пользователя {UserId}", usercontextmodel.Id);
                return new ServiceResponse<ApplicationsMetricsDto>
                {
                    Data = null,
                    Success = false,
                    Message = ResponseMessages.UnSuccess
                };
            }
        }
        public async Task<ServiceResponse<PagedResponse<ApplicationDto>>> GetApplications(
           UserContextDto userContext,
           PaginationDto pagination)
        {
            try
            {
                var query = _dbcontext.Applications
                    .AsNoTracking()
                    .Include(x => x.Photos)
                    .Include(x => x.Documents)
                    .Include(x => x.Agent)
                        .ThenInclude(a => a.User)
                    .Include(x => x.VehicleCategory);

                if (!Enum.TryParse<Models.Enums.Role>(userContext.Role, true, out var role))
                {
                    return new ServiceResponse<PagedResponse<ApplicationDto>>
                    {
                        Data = null,
                        Success = false,
                        Message = "Неизвестная роль пользователя"
                    };
                }

                IQueryable<Application> filteredQuery = role switch
                {
                    Models.Enums.Role.Admin => query,
                    Models.Enums.Role.Agent => query.Where(x => x.Agent != null && x.Agent.UserId == userContext.Id),
                    Models.Enums.Role.Moderator => query.Where(x => x.Status == ApplicationStatus.Moderated.ToString()),
                    _ => query.Where(x => false)
                };

                if (filteredQuery == null)
                {
                    return new ServiceResponse<PagedResponse<ApplicationDto>>
                    {
                        Data = null,
                        Success = false,
                        Message = "У вас нет прав для просмотра этой информации"
                    };
                }

                var totalCount = await filteredQuery.CountAsync();

                var items = await filteredQuery
                    .OrderByDescending(x => x.Id)
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
                    Success = true,
                    Message = ResponseMessages.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении заявок для пользователя {UserId}", userContext.Id);
                return new ServiceResponse<PagedResponse<ApplicationDto>>
                {
                    Data = null,
                    Success = false,
                    Message = ResponseMessages.UnSuccess
                };
            }
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
