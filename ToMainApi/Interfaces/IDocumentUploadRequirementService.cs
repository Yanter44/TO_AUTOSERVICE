using ToMainApi.Common;
using ToMainApi.Models.Dtos.Requirementss;

namespace ToMainApi.Interfaces
{
    public interface IDocumentUploadRequirementService
    {
        Task<ServiceResponse<List<DocumentRequireDto>>> GetAllDocumentRequirements();
        Task<ServiceResponse<DocumentRequireDto>> AddDocumentRequirement(AddDocumentRequirementDto model);
        Task<ServiceResponse<bool>> DeleteDocumentRequirement(int documentRequireId);
    }
}
