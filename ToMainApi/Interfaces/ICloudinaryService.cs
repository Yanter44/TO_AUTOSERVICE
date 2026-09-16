using ToMainApi.Common;
using ToMainApi.Models.Cloudinary;
using ToMainApi.Models.Enums;

namespace ToMainApi.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<string> UploadImageAsync(IFormFile file);
        Task<ServiceResponse<string>> UploadImageAsync(string base64Image);
        Task<ServiceResponse<bool>> DeleteImageByUrl(string photoUrl);
        Task<ServiceResponse<MemoryStream>> DownloadPhotoAsStreamAsync(string photoUrl);
        Task<CloudinarySignatureResponse> GenerateSignatureAsync(int userId, CloudinarySignatureRequest request);
    }
}
