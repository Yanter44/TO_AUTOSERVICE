//using MassTransit;
//using Microsoft.EntityFrameworkCore;
//using Shared.ApplicationContracts;
//using ToMainApi.DbContext;
//using ToMainApi.Models.Entities;
//using ToMainApi.Models.Enums;

//namespace ToMainApi.Services.BackgorundServices
//{
//    public class ApplicationPublisherWorker : BackgroundService
//    {
//        private readonly IServiceScopeFactory _scopeFactory;
//        private readonly ILogger<ApplicationPublisherWorker> _logger;

//        public ApplicationPublisherWorker(IServiceScopeFactory scopeFactory, ILogger<ApplicationPublisherWorker> logger)
//        {
//            _scopeFactory = scopeFactory;
//            _logger = logger;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                using var scope = _scopeFactory.CreateScope();
//                var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//                var bus = scope.ServiceProvider.GetRequiredService<IBus>();

//                // Ищем заявки с готовыми фото
//                var readyApplications = await dbcontext.Applications
//                    .Where(x => x.Status == ApplicationStatus.AllReady.ToString())
//                    .Include(x => x.ProvidedDocument)
//                    .Include(x => x.Photos) 
//                    .Include(x => x.VehicleCategoryBySrts)
//                    .Include(x => x.VehicleCategoryByOkp)
//                    .Include(x => x.Pto)
//                    .Take(10)
//                    .ToListAsync(stoppingToken);

//                foreach (var app in readyApplications)
//                {
//                    var mappedPhotos = app.Photos.Where(p => !string.IsNullOrEmpty(p.ProcessedUrl)) 
//                                                 .Select(p => new Shared.ApplicationContracts.ApplicationPhoto
//                                                 {
//                                                    Url = p.ProcessedUrl
//                                                 })                   
//                                                 .ToList();
//                    var applicationSelectedPto = await dbcontext.Ptos.Where(x => x.Id == app.PtoId)
//                                                               .Include(x => x.Experts)
//                                                               .ThenInclude(x => x.CryptoKey).FirstOrDefaultAsync();

//                    var applicationData = new ApplicationData()
//                    {
//                        Id = app.Id,
//                        ProvidedDocument = app.ProvidedDocument.DisplayName,
//                        ProvidedDocumentSeriesAndNumber = app.ProvidedDocumentSeriesAndNumber,
//                        ProvidedDocumentWhenWasIssued = app.ProvidedDocumentWhenWasIssued,
//                        ProvidedDocumentIssuedBy = app.ProvidedDocumentIssuedBy,

//                        IsVinMissing = app.IsVinMissing,
//                        VIN = app.VIN,
//                        VehicleYearOfRelease = app.VehicleYearOfRelease,
//                        Brand = app.Brand,
//                        Model = app.Model,
//                        GosNumber = app.GosNumber,
//                        VehicleBodyNumber = app.VehicleBodyNumber,
//                        IsChassisNumberMissing = app.IsChassisNumberMissing,
//                        ChassisNumber = app.ChassisNumber,
//                        AdditionalMarking = app.AdditionalMarking,
//                        MassWithoutLoad = app.MassWithoutLoad,

//                        VehicleCategoryBySrts = app.VehicleCategoryBySrts.Name,
//                        VehicleCategoryByOkp = app.VehicleCategoryByOkp.Name,

//                        VehicleMileage = app.VehicleMileage,
//                        AlowedMaximumMass = app.AlowedMaximumMass,
//                        FuelType = app.FuelType,
//                        TypeOfBrakeSystemDrive = app.TypeOfBrakeSystemDrive,
//                        TireBrand = app.TireBrand,
//                        SpecialPurpose = app.SpecialPurpose,

//                        // Tahograph
//                        IsTahogramMissing = app.IsTahogramMissing,
//                        BrandOfTachograph = app.BrandOfTachograph,
//                        ModelOfTachograph = app.ModelOfTachograph,
//                        SerialNumberOfTachograph = app.SerialNumberOfTachograph,

//                        // GasEquipment
//                        IsGasCylinderEquipmentMissing = app.IsGasCylinderEquipmentMissing,
//                        NumberOfGasCylinderEquipment = app.NumberOfGasCylinderEquipment,
//                        DateOfTheNextInspectionGasCylinderEquipment = app.DateOfTheNextInspectionGasCylinderEquipment,


//                        //GasСylinder
//                        IsGasCylinderMissing = app.IsGasCylinderMissing,
//                        GasCylinderYearOfRelease = app.GasCylinderYearOfRelease,
//                        GasCylinderSerialNumber = app.GasCylinderSerialNumber,
//                        DateOfTheLastExaminationGasCylinder = app.DateOfTheLastExaminationGasCylinder,
//                        DateOfTheNextInspectionGasCylinder = app.DateOfTheNextInspectionGasCylinder,
                        
//                        // Owner
//                        FIO = app.FIO,
//                        Email = app.Email,
//                        PhoneNumber = app.PhoneNumber,

//                        Photos = mappedPhotos,

//                        PtoId = app.PtoId,
//                        SelectedPtoExpertId = app.SelectedTechnicalExpertToReviewApplicationId,

//                        PtoExpertCryptoKeyEncryptedData = await dbcontext.PtoExpertCryptoKeys
//                                                                         .Where(x => x.ExpertId == app.SelectedTechnicalExpertToReviewApplicationId)
//                                                                         .Select(x => x.EncryptedData)
//                                                                         .FirstOrDefaultAsync(),
//                       // PtoExpertCryptoKeyName
                        
//                        CreatedAt = app.CreatedAt
//                    };

//                    var message = new SubmitApplication(app.Id, applicationData);
//                    await bus.SubmitJob(message, stoppingToken);
//                    _logger.LogInformation("Заявка {Id} отправлена в Job Service", app.Id);
//                    app.Status = ApplicationStatus.EaistoProcessing.ToString();
//                }

//                await dbcontext.SaveChangesAsync(stoppingToken);
//                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
//            }
//        }
//    }
//}