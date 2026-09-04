using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Npgsql.BackendMessages;
using System.Net;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using ToMainApi.Interfaces;
using ToMainApi.Models.Cloudinary;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private Cloudinary _cloudinary;
        private readonly HttpClient _httpclient;
        private readonly IConfiguration _configuration;
        public CloudinaryService(IConfiguration configuration, HttpClient httpclient)
        {
            _configuration = configuration;

            var account = new Account(
                _configuration["CloudinaryService:Cloud"],
                _configuration["CloudinaryService:ApiKey"],
                _configuration["CloudinaryService:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
            _httpclient = httpclient;
        }
        public async Task<CloudinarySignatureResponse> GenerateSignatureAsync(int userId, CloudinarySignatureRequest request)
        {
            var blockedTypes = new[] {
                "application/x-msdownload",
                "application/x-executable",
                "text/html",
                "application/javascript",
                "application/x-php",
                "application/x-msdos-program",
                "application/java-archive"
            };

            if (blockedTypes.Any(b => request.FileType.Contains(b, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("Загрузка файлов этого типа запрещена");
            }

            const long maxSize = 10 * 1024 * 1024;
            if (request.FileSize > maxSize)
            {
                throw new Exception($"Файл слишком большой. Максимум {maxSize / 1024 / 1024} MB");
            }

            var cloudName = _configuration["CloudinaryService:Cloud"];
            var apiKey = _configuration["CloudinaryService:ApiKey"];
            var apiSecret = _configuration["CloudinaryService:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            var cloudinary = new Cloudinary(account);

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var folder = $"applications/{userId}/{DateTime.UtcNow:yyyy-MM-dd}";
            var uploadPreset = "agentUploads";

            var parameters = new Dictionary<string, object>
            {
                { "timestamp", timestamp },
                { "upload_preset", uploadPreset },
            };

            var signature = cloudinary.Api.SignParameters(parameters);

            return new CloudinarySignatureResponse
            {
                Signature = signature,
                Timestamp = timestamp,
                ApiKey = apiKey,
                CloudName = cloudName,
                Folder = folder,
                UploadPreset = uploadPreset,
                MaxFileSize = maxSize
            };
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var uploadParams = new RawUploadParams()
            {
                Folder = "applications/documents",
                File = new FileDescription(fileName, stream)
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }

            throw new Exception("Ошибка загрузки документа");
        }
        public async Task<MemoryStream> DownloadPhotoAsStreamAsync(string photoUrl)
        {
            var response = await _httpclient.GetAsync(photoUrl);
            response.EnsureSuccessStatusCode();

            var stream = new MemoryStream();

            await response.Content.CopyToAsync(stream);

            stream.Position = 0;

            return stream;
        }
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var uploadParams = new ImageUploadParams()
            {
                Folder = "applications/photos",
                File = new FileDescription(fileName, stream)
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }

            throw new Exception("Ошибка загрузки изображения");
        }
    }
}
