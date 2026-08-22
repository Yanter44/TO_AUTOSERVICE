using ToMainApi.Common;
using ToMainApi.Models.Dtos.ApplicationPhoto;

namespace ToMainApi.Interfaces
{
    public interface IApplicationPhotoService
    {
        Task<ServiceResponse<List<ApplicationPhotoDto>>> GetAllApplicationPhotos();
        Task<ServiceResponse<ApplicationPhotoDto>> GetApplicationPhotoByApplicationIdAndPhotoId(int applicationId, int photoId);
        Task<ServiceResponse<List<ApplicationPhotoDto>>> GetApplicationPhotosByApplicationId(int applicationId);
    }
}
