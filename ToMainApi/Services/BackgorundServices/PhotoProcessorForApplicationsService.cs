//using Microsoft.EntityFrameworkCore;
//using ToMainApi.DbContext;
//using ToMainApi.Interfaces;
//using ToMainApi.Models.Enums;

//namespace ToMainApi.Services.BackgorundServices
//{
//    public class PhotoProcessorForApplicationsService : BackgroundService
//    {
//        private readonly IServiceScopeFactory _serviceScopeFactory;
//        private readonly ILogger<PhotoProcessorForApplicationsService> _logger;
//        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(15);

//        public PhotoProcessorForApplicationsService(ILogger<PhotoProcessorForApplicationsService> logger, IServiceScopeFactory scopeFactory)
//        {
//            _logger = logger;
//            _serviceScopeFactory = scopeFactory;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                try
//                {
//                    using var scope = _serviceScopeFactory.CreateScope();
//                    var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//                    var _metadataEditorService = scope.ServiceProvider.GetRequiredService<IimageMetadataEditor>();
//                    var _imagetextoverlayservice = scope.ServiceProvider.GetRequiredService<IimageTextOverlayService>();
//                    var _cloudinaryService = scope.ServiceProvider.GetRequiredService<ICloudinaryService>();

//                    var approvedApplications = await dbcontext.Applications
//                        .Where(x => x.Status == ApplicationStatus.Approved.ToString())
//                        .Include(x => x.Pto)
//                        .Take(5) // Берем пачками, чтобы не грузить сервер
//                        .ToListAsync(stoppingToken);

//                    foreach (var application in approvedApplications)
//                    {
//                        bool isAllProcessed = true;
//                        var applicationPhotos = await dbcontext.ApplicationPhotos
//                            .Where(x => x.ApplicationId == application.Id)
//                            .ToListAsync(stoppingToken);

//                        foreach (var applicationPhoto in applicationPhotos)
//                        {
//                            try
//                            {
//                                var download = await _cloudinaryService.DownloadPhotoAsStreamAsync(applicationPhoto.Url);
//                                if (!download.Success || download.Data is null)
//                                {
//                                    _logger.LogWarning("Не удалось скачать фото {Url}", applicationPhoto.Url);
//                                    isAllProcessed = false;
//                                    break;
//                                }

//                                await using var photostream = download.Data;
//                                var metadataEditorResult = await _metadataEditorService.ProcessImage(photostream);
//                                if (!metadataEditorResult.Success)
//                                {
//                                    isAllProcessed = false;
//                                    break;
//                                }

//                                var firstLine = $"{application.Pto.Latitude};{application.Pto.Longitude}";
//                                var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(application.CreatedAt, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"));
//                                var secondLine = moscowTime.ToString("dd.MM.yyyy HH:mm");

//                                var result = await _imagetextoverlayservice.AddText(metadataEditorResult.Data, new List<string> { firstLine, secondLine });
//                                if (!result.Success || result.Data == null)
//                                {
//                                    isAllProcessed = false;
//                                    break;
//                                }
//                                using var ms = new MemoryStream();
//                                await result.Data.CopyToAsync(ms, stoppingToken);
//                                var base64Image = Convert.ToBase64String(ms.ToArray());
//                                var uploadResult = await _cloudinaryService.UploadImageAsync(base64Image);
//                                if (!uploadResult.Success)
//                                {
//                                    isAllProcessed = false;
//                                    break;
//                                }
//                                applicationPhoto.ProcessedUrl = uploadResult.Data;
//                                _logger.LogInformation("Фото {PhotoId} успешно обработано", applicationPhoto.Id);
//                            }
//                            catch (Exception ex)
//                            {
//                                _logger.LogError(ex, "Ошибка при обработке фото {PhotoId}", applicationPhoto.Id);
//                                isAllProcessed = false;
//                                break;
//                            }
//                        }
//                        if (isAllProcessed && applicationPhotos.Any())
//                        {
//                            application.Status = ApplicationStatus.AllReady.ToString(); 
//                            _logger.LogInformation("Все фото для заявки {Id} готовы к отправке в RabbitMQ", application.Id);
//                        }

//                        await dbcontext.SaveChangesAsync(stoppingToken);
//                    }
//                    await Task.Delay(Interval, stoppingToken);
//                }
//                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
//                {
//                    break;
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Ошибка в фоновой обработке ЕАИСТО");
//                }
//            }
//        }
//    }
//}