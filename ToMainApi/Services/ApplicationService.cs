using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Features.Events.Applications;
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
        private readonly IMediator _mediator;
        private readonly ILogger<ApplicationService> _logger;
        public ApplicationService(AppDbContext dbcontext, IMediator mediator,
            ILogger<ApplicationService> logger)
        {
            _dbcontext = dbcontext;
            _mediator = mediator;
            _logger = logger;

        }
        public async Task<ServiceResponse<ApplicationDto>> CreateNewApplication(UserContextDto userContext, CreateNewApplicationDto model)
        {
            var agentId = await _dbcontext.AgentProfiles
                .Where(ap => ap.UserId == userContext.Id)
                .Select(ap => ap.Id)
                .FirstOrDefaultAsync();

            if (agentId == 0)
                return new ServiceResponse<ApplicationDto> { Success = false, Message = "Профиль агента не найден в системе" };
       
            var application = new Application
            {
                AgentId = agentId,

                ProvidedDocumentId = model.ProvidedDocumentId,
                ProvidedDocumentSeriesAndNumber = model.ProvidedDocumentSeriesAndNumber,
                ProvidedDocumentWhenWasIssued = model.ProvidedDocumentWhenWasIssued,
                ProvidedDocumentIssuedBy = model.ProvidedDocumentIssuedBy,

                IsVinMissing = model.IsVinMissing,
                VIN = model.VIN,
                VehicleYearOfRelease = model.VehicleYearOfRelease,
                Brand = model.Brand,
                Model = model.Model,
                GosNumber = model.GosNumber,
                VehicleBodyNumber = model.VehicleBodyNumber,
                IsChassisNumberMissing = model.IsChassisNumberMissing,
                ChassisNumber = model.ChassisNumber,
                AdditionalMarking = model.AdditionalMarking,
                MassWithoutLoad = model.MassWithoutLoad,
                VehicleCategoryBySrtsId = model.VehicleCategoryBySrtsOrPtsId,
                VehicleCategoryByOkpId = model.VehicleCategoyByOkpId,
                VehicleMileage = model.VehicleMileage,
                AlowedMaximumMass = model.AlowedMaximumMass,
                FuelType = model.FuelType,
                TypeOfBrakeSystemDrive = model.TypeOfBrakeSystemDrive,
                TireBrand = model.TireBrand,
                SpecialPurpose = model.SpecialPurpose,
                IsTahogramMissing = model.IsTahogramMissing,
                BrandOfTachograph = model.BrandOfTachograph,
                ModelOfTachograph = model.ModelOfTachograph,
                SerialNumberOfTachograph = model.SerialNumberOfTachograph,
                IsGasCylinderEquipmentMissing = model.IsGasCylinderEquipmentMissing,
                NumberOfGasCylinderEquipment = model.NumberOfGasCylinderEquipment,
                DateOfTheNextInspectionGasCylinderEquipment = model.DateOfTheNextInspectionGasCylinderEquipment,

                IsGasCylinderMissing = model.IsGasCylinderMissing,
                GasCylinderYearOfRelease = model.GasCylinderYearOfRelease,
                GasCylinderSerialNumber = model.GasCylinderSerialNumber,
                DateOfTheLastExaminationGasCylinder = model.DateOfTheLastInspectionGasCylinder,
                DateOfTheNextInspectionGasCylinder = model.DateOfTheNextInspectionGasCylinder,

                FIO = model.FIO,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,

                PtoId = model.PtoId,
                Photos = model.VehiclePhotos.Select(photo => new ApplicationPhoto
                {
                    VehiclePhotoType = photo.VehiclePhotoType,
                    Url = photo.PhotoUrl
                }).ToList(),
                Status = ApplicationStatus.OnModeration.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            _dbcontext.Applications.Add(application);
            await _dbcontext.SaveChangesAsync();
            await _mediator.Publish(new ApplicationCreatedEvent(application.Id, userContext.Id.Value));

            return new ServiceResponse<ApplicationDto>
            {
                Success = true,
                Message = "Заявка создана",
                Data = new ApplicationDto
                {
                    Id = application.Id,
                    VIN = application.VIN,
                    GosNumber = application.GosNumber,
                    Brand = application.Brand,
                    Model = application.Model,
                    FIO = application.FIO,
                    Email = application.Email,
                    PhoneNumber = application.PhoneNumber,
                    PtoId = application.PtoId,
                    CreatedAt = application.CreatedAt,
                    Status = application.Status,
                    Photos = application.Photos.Select(photo => new ApplicationPhotoDto
                    {
                        Id = photo.Id,
                        FileName = $"photo_{photo.Id}.jpg",
                        VehiclePhotoType = photo.VehiclePhotoType,
                        Url = photo.Url
                    }).ToList()
                }
            };
        }
        public async Task<ServiceResponse<bool>> AcceptApplication(AcceptApplicationDto model)
        {
            var existApplication = await _dbcontext.Applications.FirstOrDefaultAsync(x => x.Id == model.ApplicationId);
            if (existApplication == null)
            {
                return new ServiceResponse<bool>() { Success = false, Message = "Такой заявки нет в системе" };
            }
            existApplication.Status = ApplicationStatus.Approved.ToString();
            await _dbcontext.SaveChangesAsync();
            return new ServiceResponse<bool>() { Success = true, Message = ResponseMessages.Success };
        }
        public async Task<ServiceResponse<bool>> RejectApplication(RejectApplicationDto model)
        {
            var existApplication = await _dbcontext.Applications.FirstOrDefaultAsync(x => x.Id == model.ApplicationId);
            if(existApplication == null)
            {
                return new ServiceResponse<bool>() { Success = false, Message = "Такой заявки нет в системе" };
            }
            existApplication.Status = ApplicationStatus.RejectedByModerator.ToString();
            existApplication.RejectReason = model.RejectReason;
            await _dbcontext.SaveChangesAsync();
            return new ServiceResponse<bool>() { Success = true, Message = ResponseMessages.Success };
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
                        InModeration = g.Count(x => x.Status == ApplicationStatus.OnModeration.ToString()),
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
        public async Task<ServiceResponse<PagedResponse<ApplicationDto>>> GetApplications(UserContextDto userContext, PaginationDto pagination)
        {
            try
            {
                if (!Enum.TryParse<Models.Enums.Role>(userContext.Role, true, out var role))
                {
                    return new ServiceResponse<PagedResponse<ApplicationDto>>
                    {
                        Data = null,
                        Success = false,
                        Message = "Неизвестная роль пользователя"
                    };
                }

                var query = _dbcontext.Applications.AsNoTracking()
                    .Include(x => x.ProvidedDocument)
                    .Include(x => x.VehicleCategoryBySrts)
                    .Include(x => x.VehicleCategoryByOkp);

                var filteredQuery = role switch
                {
                    Models.Enums.Role.Admin => query,
                    Models.Enums.Role.Moderator => query,
                    Models.Enums.Role.Agent => query.Where(x => x.Agent.UserId == userContext.Id),
                    _ => query.Where(x => false)
                };

                var totalCount = await filteredQuery.CountAsync();

              

                var items = await filteredQuery
                    .OrderByDescending(x => x.Id)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .Select(application => new ApplicationDto
                    {
                        Id = application.Id,

                        ProvidedDocumentName = application.ProvidedDocument.DisplayName,
                        ProvidedDocumentSeriesAndNumber = application.ProvidedDocumentSeriesAndNumber,
                        ProvidedDocumentWhenWasIssued = application.ProvidedDocumentWhenWasIssued,
                        ProvidedDocumentIssuedBy = application.ProvidedDocumentIssuedBy,

                        IsVinMissing = application.IsVinMissing,
                        VIN = application.VIN,
                        VehicleYearOfRelease = application.VehicleYearOfRelease,
                        Brand = application.Brand,
                        Model = application.Model,
                        GosNumber = application.GosNumber,
                        VehicleBodyNumber = application.VehicleBodyNumber,
                        IsChassisNumberMissing = application.IsChassisNumberMissing,
                        ChassisNumber = application.ChassisNumber,
                        AdditionalMarking = application.AdditionalMarking,
                        MassWithoutLoad = application.MassWithoutLoad,
                        VehicleCategoryBySrts = application.VehicleCategoryBySrts.Name,
                        VehicleCategoyByOkp = application.VehicleCategoryByOkp.Name,
                        VehicleMileage = application.VehicleMileage,
                        AlowedMaximumMass = application.AlowedMaximumMass,
                        FuelType = application.FuelType,
                        TypeOfBrakeSystemDrive = application.TypeOfBrakeSystemDrive,
                        TireBrand = application.TireBrand,
                        SpecialPurpose = application.SpecialPurpose,
                        IsTahogramMissing = application.IsTahogramMissing,
                        BrandOfTachograph = application.BrandOfTachograph,
                        ModelOfTachograph = application.ModelOfTachograph,
                        SerialNumberOfTachograph = application.SerialNumberOfTachograph,
                        IsGasCylinderEquipmentMissing = application.IsGasCylinderEquipmentMissing,
                        NumberOfGasCylinderEquipment = application.NumberOfGasCylinderEquipment,
                        DateOfTheNextInspectionGasCylinderEquipment = application.DateOfTheNextInspectionGasCylinderEquipment,

                        IsGasCylinderMissing = application.IsGasCylinderMissing,
                        GasCylinderYearOfRelease = application.GasCylinderYearOfRelease,
                        GasCylinderSerialNumber = application.GasCylinderSerialNumber,
                        DateOfTheLastInspectionGasCylinder = application.DateOfTheLastExaminationGasCylinder,
                        DateOfTheNextInspectionGasCylinder = application.DateOfTheNextInspectionGasCylinder,
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
                        }).ToList()
                    })
                    .ToListAsync();

                return new ServiceResponse<PagedResponse<ApplicationDto>>
                {
                    Data = new PagedResponse<ApplicationDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        Page = pagination.Page,
                        PageSize = pagination.PageSize,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize)
                    },
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
                    Success = false,
                    Message = "Заявка не найдена в системе"
                };
            }
            _dbcontext.Applications.Remove(existApplication);
            await _dbcontext.SaveChangesAsync();
            return new ServiceResponse<bool>
            {
                Success = true,
                Message = ResponseMessages.Success
            };
        }

    }
}
