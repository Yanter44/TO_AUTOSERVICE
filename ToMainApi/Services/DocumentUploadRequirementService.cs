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
        public async Task<ServiceResponse<List<DocumentRequireDto>>> GetAllDocumentRequirements()
        {
            var allrequirements = await _dbContext.DocumentUploadRequires.AsNoTracking().ToListAsync();
            var dtos = allrequirements.Select(x => new DocumentRequireDto
            {
                Id = x.Id,
                DocumentType = x.DocumentType,
                DisplayName = x.DisplayName,
                IsRequire = x.IsRequire
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
            var newRequirement = new DocumentUploadRequire
            {
                DocumentType = model.DocumentType.Trim(),
                DisplayName = model.DisplayName.Trim(),
                IsRequire = model.IsRequire
            };

            await _dbContext.DocumentUploadRequires.AddAsync(newRequirement);
            await _dbContext.SaveChangesAsync();

            var resultDto = new DocumentRequireDto
            {
                Id = newRequirement.Id,
                DocumentType = newRequirement.DocumentType,
                DisplayName = newRequirement.DisplayName,
                IsRequire = newRequirement.IsRequire
            };

            return new ServiceResponse<DocumentRequireDto>
            {
                Data = resultDto,
                Success = true,
            };
        }
        public async Task<ServiceResponse<bool>> DeleteDocumentRequirement(int documentRequireId)
        {
            var existrequirement = await _dbContext.DocumentUploadRequires.FirstOrDefaultAsync(x => x.Id == documentRequireId);
            if(existrequirement != null)
            {
                _dbContext.Remove(existrequirement);
                await _dbContext.SaveChangesAsync();
                return new ServiceResponse<bool>() { Success = true };
            }
            return new ServiceResponse<bool>() { Success = false };
        } 
    }
}
