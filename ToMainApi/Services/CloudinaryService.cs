using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Npgsql.BackendMessages;
using System.Net;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using ToMainApi.Common;
using ToMainApi.Interfaces;
using ToMainApi.Models.Cloudinary;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private Cloudinary _cloudinary;
        private readonly HttpClient _httpclient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CloudinaryService> _logger;
        public CloudinaryService(IConfiguration configuration,
            HttpClient httpclient,
            ILogger<CloudinaryService> logger)
        {
            _configuration = configuration;

            var account = new Account(
                _configuration["CloudinaryService:Cloud"],
                _configuration["CloudinaryService:ApiKey"],
                _configuration["CloudinaryService:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
            _httpclient = httpclient;
            _logger = logger;
        }

        public async Task<CloudinarySignatureResponse> GenerateSignatureAsync(UserContextDto userContext, UploadPhotoScenario scenario)
        {
            var p = BuildUploadParams(userContext, scenario);

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var parameters = new Dictionary<string, object>
            {
                { "folder", p.Folder },
                { "timestamp", timestamp },
            };

            var signature = _cloudinary.Api.SignParameters(parameters);

            return new CloudinarySignatureResponse
            {
                Signature = signature,
                Timestamp = timestamp,
                ApiKey = _configuration["CloudinaryService:ApiKey"],
                CloudName = _configuration["CloudinaryService:Cloud"],
                Folder = p.Folder,
                UploadPreset = p.UploadPreset
            };
        }

        private static CloudinaryUploadParams BuildUploadParams(UserContextDto user, UploadPhotoScenario scenario)
        {
            var date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            return scenario switch
            {
                UploadPhotoScenario.AgentUploadApplicationPhoto => new CloudinaryUploadParams
                {
                    Folder = $"applicationPhotos/{user.Id}/photos",
                },
                UploadPhotoScenario.AdminUploadLibraryPhoto => new CloudinaryUploadParams
                {
                    Folder = $"photoLibrary/photos",
                },
                _ => throw new ArgumentOutOfRangeException(
                    nameof(scenario), $"Неизвестный сценарий: {scenario}")
            };
        }
        public async Task<ServiceResponse<bool>> DeleteImageByUrl(string photoUrl)
        {
            if (string.IsNullOrWhiteSpace(photoUrl))
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "URL фото пустой"
                };

            try
            {
                var publicId = ExtractPublicId(photoUrl);

                if (string.IsNullOrWhiteSpace(publicId))
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Не удалось определить publicId из URL"
                    };

                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image
                };

                var result = await _cloudinary.DestroyAsync(deletionParams);

                if (result.Result == "ok")
                    return new ServiceResponse<bool> { Success = true, Data = true };

                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Cloudinary вернул: {result.Result}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления фото: {Url}", photoUrl);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Ошибка при удалении фото"
                };
            }
        }

        public async Task<ServiceResponse<MemoryStream>> DownloadPhotoAsStreamAsync(string photoUrl)
        {
            if (string.IsNullOrWhiteSpace(photoUrl))
                return new ServiceResponse<MemoryStream>
                {
                    Success = false,
                    Message = "URL фото пустой"
                };

            try
            {
                var response = await _httpclient.GetAsync(photoUrl);
                response.EnsureSuccessStatusCode();

                var stream = new MemoryStream();
                await response.Content.CopyToAsync(stream);
                stream.Position = 0;

                return new ServiceResponse<MemoryStream>
                {
                    Success = true,
                    Data = stream
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при скачивании фото: {Url}", photoUrl);
                return new ServiceResponse<MemoryStream>
                {
                    Success = false,
                    Message = "Ошибка при скачивании фото"
                };
            }
        }

        public async Task<ServiceResponse<string>> UploadImageAsync(string base64Image)
        {
            try
            {
                var base64Data = base64Image.Contains(",")
                    ? base64Image.Substring(base64Image.IndexOf(",") + 1)
                    : base64Image;

                var bytes = Convert.FromBase64String(base64Data);
                using var stream = new MemoryStream(bytes);

                var uploadParams = new ImageUploadParams()
                {
                    Folder = "applications/photos",
                    File = new FileDescription("generated.png", stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                    return new ServiceResponse<string>()
                    {
                        Data = uploadResult.SecureUrl.ToString(),
                        Success = true,
                        Message = ResponseMessages.Success
                    };
                }

                return new ServiceResponse<string>() { Data = null, Success = false, Message = "Что-то пошло не так при попытке загрузить фото" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при попытке загрузить фото");
                return new ServiceResponse<string>() { Data = null, Success = false, Message = "Не удалось загрузить фото" };
            }
        }
        private static string? ExtractPublicId(string url)
        {
            var marker = "/upload/";
            var idx = url.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;

            var path = url[(idx + marker.Length)..];
            if (path.StartsWith("v"))
            {
                var slash = path.IndexOf('/');
                if (slash > 0 && long.TryParse(path[1..slash], out _))
                    path = path[(slash + 1)..];
            }
            var dot = path.LastIndexOf('.');
            if (dot > 0) path = path[..dot];

            return path;
        }
    }
}
