using ToMainApi.Models.Cloudinary;
using ToMainApi.Models.Enums;

namespace ToMainApi.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<string> UploadImageAsync(IFormFile file);
        Task<MemoryStream> DownloadPhotoAsStreamAsync(string photoUrl);
        Task<CloudinarySignatureResponse> GenerateSignatureAsync(int userId, CloudinarySignatureRequest request);
    }
}
