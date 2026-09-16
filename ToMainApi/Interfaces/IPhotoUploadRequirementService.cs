using ToMainApi.Common;
using ToMainApi.Models.Dtos.Requirementss;

namespace ToMainApi.Interfaces
{
    public interface IPhotoUploadRequirementService
    {
        Task<ServiceResponse<List<PhotoRequireDto>>> GetAllPhotoRequirements();
        Task<ServiceResponse<PhotoRequireDto>> AddPhotoRequirement(AddPhotoRequirementDto model);
        Task<ServiceResponse<PhotoRequireDto>> EditPhotoRequirement(EditPhotoRequirementDto model);
        Task<ServiceResponse<bool>> DeletePhotoRequirement(int photorequirementId);
    }
}
