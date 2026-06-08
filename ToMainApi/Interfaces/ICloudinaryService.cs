using ToMainApi.Models.Enums;

namespace ToMainApi.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<string> UploadImageAsync(IFormFile file);
    }
}
