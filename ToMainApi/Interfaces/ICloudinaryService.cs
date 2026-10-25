using ToMainApi.Common;
using ToMainApi.Models.Cloudinary;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Enums;
namespace ToMainApi.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ServiceResponse<string>> UploadImageAsync(string base64Image);
        Task<ServiceResponse<bool>> DeleteImageByUrl(string photoUrl);
        Task<ServiceResponse<MemoryStream>> DownloadPhotoAsStreamAsync(string photoUrl);
        Task<CloudinarySignatureResponse> GenerateSignatureAsync(UserContextDto userContext, UploadPhotoScenario scenario);
    }
}
