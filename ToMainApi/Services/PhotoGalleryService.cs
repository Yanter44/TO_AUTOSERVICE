using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.PhotoGallery;
using ToMainApi.Models.Dtos.Prompt;
using ToMainApi.Models.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ToMainApi.Services
{
    public class PhotoGalleryService  : IPhotoGalleryService
    {
        private readonly AppDbContext _dbcontext;
        private readonly ILogger<PhotoGalleryService> _logger;
        private readonly ICloudinaryService _cloudinary;
        public PhotoGalleryService(AppDbContext dbContext,
            ILogger<PhotoGalleryService> logger, 
            ICloudinaryService cloudinary)
        {
            _dbcontext = dbContext;
            _logger = logger;
            _cloudinary = cloudinary;
        }
        public async Task<ServiceResponse<PagedResponse<PhotoInGalleryDto>>> GetPhotoGallery(PaginationDto paginationModel)
        {
            var query = _dbcontext.PhotoGallery.AsNoTracking()
                .Select(x => new PhotoInGalleryDto
                {
                    Id = x.Id,
                    Tag = x.Tag,
                    Group = x.Group,
                    Url = x.Url,
                    CreatedAt = x.CreatedAt,
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((paginationModel.Page - 1) * paginationModel.PageSize)
                .Take(paginationModel.PageSize)
                .ToListAsync();

            return new ServiceResponse<PagedResponse<PhotoInGalleryDto>>
            {
                Data = new PagedResponse<PhotoInGalleryDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = paginationModel.Page,
                    PageSize = paginationModel.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / paginationModel.PageSize)
                },
                Success = true
            };
        }
        public async Task<ServiceResponse<bool>> DeletePhotoFromPhotoGallery(int photoId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var photo = await _dbcontext.PhotoGallery
                    .FirstOrDefaultAsync(x => x.Id == photoId);

                if (photo is null)
                {
                    response.Success = false;
                    response.Message = "Фото не найдено";
                    return response;
                }
                if (!string.IsNullOrWhiteSpace(photo.PublicId))
                {
                    try
                    {
                        var deleteResult = await _cloudinary.DeleteImageByUrl(photo.Url);
                        if (deleteResult.Success != true)
                        {
                            _logger.LogWarning("Cloudinary вернул неожиданный результат для {PublicId}: {Result}", photo.PublicId, deleteResult.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Не удалось удалить фото из Cloudinary: {PublicId}",
                            photo.PublicId);
                    }
                }
                _dbcontext.PhotoGallery.Remove(photo);
                await _dbcontext.SaveChangesAsync();

                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении фото {PhotoId}", photoId);
                response.Success = false;
                response.Message = "Внутренняя ошибка сервера";
                return response;
            }
        }

        public async Task<ServiceResponse<PhotoInGalleryDto>> AddNewPhotoToGallery(AddNewPhotoToGalleryDto model)
        {
            var response = new ServiceResponse<PhotoInGalleryDto>();

            try
            {
                if (string.IsNullOrWhiteSpace(model.PublicId))
                {
                    response.Success = false;
                    response.Message = "PublicId обязателен";
                    return response;
                }

                var exists = await _dbcontext.PhotoGallery
                    .AnyAsync(x => x.PublicId == model.PublicId);

                if (exists)
                {
                    response.Success = false;
                    response.Message = "Это фото уже в галерее";
                    return response;
                }

                var photo = new PhotoInGallery
                {
                    Tag = model.Tag,
                    Group = model.Group,
                    PublicId = model.PublicId,
                    Url = model.Url,
                    CreatedAt = DateTime.UtcNow
                };
                _dbcontext.PhotoGallery.Add(photo);

                try
                {
                    await _dbcontext.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (IsUniqueViolation(ex))
                {
                    response.Success = false;
                    response.Message = "Это фото уже в галерее";
                    return response;
                }

                return new ServiceResponse<PhotoInGalleryDto>() 
                {
                    Data = new PhotoInGalleryDto
                    {
                        Id = photo.Id,
                        Tag = photo.Tag,
                        Group = photo.Group,
                        PublicId = photo.PublicId,
                        Url = photo.Url,
                        CreatedAt = photo.CreatedAt
                    },
                    Success = true,
                    Message = "Успешно добавили новое изображение"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении фото в галерею");
                response.Success = false;
                response.Message = "Внутренняя ошибка сервера";
                return response;
            }
        }
        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            return ex.InnerException is Npgsql.PostgresException pg && pg.SqlState == "23505";
        }
    }
}
