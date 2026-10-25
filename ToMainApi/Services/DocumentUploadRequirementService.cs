using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Requirementss;
using ToMainApi.Models.Entities;

namespace ToMainApi.Services
{
    public class DocumentUploadRequirementService : IDocumentUploadRequirementService
    {
        private readonly AppDbContext _dbContext;
        public DocumentUploadRequirementService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceResponse<DocumentRequireDto>> EditDocumentRequirement(EditDocumentRequirementDto model)
        {
            var existRequirement = await _dbContext.ApplicationProvidedDocuments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
            if(existRequirement != null)
            {
                existRequirement.DisplayName = model.DisplayName;
                existRequirement.DocumentType = model.DocumentType;
                await _dbContext.SaveChangesAsync();
                var returnmodel = new DocumentRequireDto()
                {
                    Id = existRequirement.Id,
                    DisplayName = existRequirement.DisplayName,
                    DocumentType = model.DocumentType,
                };
                return new ServiceResponse<DocumentRequireDto>()
                {
                    Data = returnmodel,
                    Success = true,
                    Message = ResponseMessages.Success
                };
            }
            return new ServiceResponse<DocumentRequireDto>()
            {
                Success = false,
                Message = "Не удалось применить изменение к требованию"
            };
        }
        public async Task<ServiceResponse<List<DocumentRequireDto>>> GetAllDocumentRequirements()
        {
            var allrequirements = await _dbContext.ApplicationProvidedDocuments.AsNoTracking().ToListAsync();
            var dtos = allrequirements.Select(x => new DocumentRequireDto
            {
                Id = x.Id,
                DocumentType = x.DocumentType,
                DisplayName = x.DisplayName,
            }).ToList();
            return new ServiceResponse<List<DocumentRequireDto>>() { Data = dtos, Success = true };
        }

        public async Task<ServiceResponse<DocumentRequireDto>> AddDocumentRequirement(AddDocumentRequirementDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.DocumentType) || string.IsNullOrWhiteSpace(model.DisplayName))
            {
                return new ServiceResponse<DocumentRequireDto>
                {
                    Success = false,
                };
            }
            var isDuplicate = await _dbContext.PhotoUploadRequires.AnyAsync(x => x.PhotoType.ToLower() == model.DocumentType.ToLower());

            if (isDuplicate)
            {
                return new ServiceResponse<DocumentRequireDto>
                {
                    Success = false,
                };
            }
            var newRequirement = new ApplicationProvidedDocument
            {
                DocumentType = model.DocumentType.Trim(),
                DisplayName = model.DisplayName.Trim(),
            };

            await _dbContext.ApplicationProvidedDocuments.AddAsync(newRequirement);
            await _dbContext.SaveChangesAsync();

            var resultDto = new DocumentRequireDto
            {
                Id = newRequirement.Id,
                DocumentType = newRequirement.DocumentType,
                DisplayName = newRequirement.DisplayName,
            };

            return new ServiceResponse<DocumentRequireDto>
            {
                Data = resultDto,
                Success = true,
            };
        }
        public async Task<ServiceResponse<bool>> DeleteDocumentRequirement(int documentRequireId)
        {
            var existrequirement = await _dbContext.ApplicationProvidedDocuments.FirstOrDefaultAsync(x => x.Id == documentRequireId);
            if(existrequirement != null)
            {
                _dbContext.Remove(existrequirement);
                await _dbContext.SaveChangesAsync();
                return new ServiceResponse<bool>() { Success = true, Message = "Требование удалено" };
            }
            return new ServiceResponse<bool>() { Success = false, Message = "Требование не найдено" };
        } 
    }
}
