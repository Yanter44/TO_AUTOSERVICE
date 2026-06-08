using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Npgsql.BackendMessages;
using System.Net;
using System.Security.Principal;
using ToMainApi.Interfaces;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;
        public CloudinaryService(IConfiguration configuration)
        {
            _configuration = configuration;

            var account = new Account(
                _configuration["CloudinaryService:Cloud"],
                _configuration["CloudinaryService:ApiKey"],
                _configuration["CloudinaryService:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
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
