using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Requirementss;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequirementsController : ControllerBase
    {
        private readonly IPhotoUploadRequirementService _photoRequirementService;
        private readonly IDocumentUploadRequirementService _documentRequirementService;
        public RequirementsController(IPhotoUploadRequirementService photoRequirementService, 
            IDocumentUploadRequirementService documentRequirementService)
        {
            _photoRequirementService = photoRequirementService;
            _documentRequirementService = documentRequirementService;   
        }

        [HttpGet("GetAllPhotoRequirements")]
        public async Task<IActionResult> GetAllPhotoRequirements()
        {
            var result = await _photoRequirementService.GetAllPhotoRequirements();
            if (result.Success)
                return Ok(result.Data);
            return BadRequest();
        }
  
        [Authorize(Roles ="Admin")]
        [HttpPost("AddNewPhotoRequirement")]
        public async Task<IActionResult> AddNewPhotoRequirement([FromBody] AddPhotoRequirementDto model)
        {
            var result = await _photoRequirementService.AddPhotoRequirement(model);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest();
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("DeletePhotoRequirement")]
        public async Task<IActionResult> DeletePhotoRequirement([FromQuery] int photoRequirementId)
        {
            var result = await _photoRequirementService.DeletePhotoRequirement(photoRequirementId);
            if (result.Success)
                return Ok();
            return BadRequest();
        }

        [HttpGet("GetAllDocumentRequirements")]
        public async Task<IActionResult> GetAllDocumentRequirements()
        {
            var result = await _documentRequirementService.GetAllDocumentRequirements();
            if (result.Success)
                return Ok(result.Data);
            return BadRequest();
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("AddNewDocumentRequirement")]
        public async Task<IActionResult> AddNewDocumentRequirement([FromBody] AddDocumentRequirementDto model)
        {
            var result = await _documentRequirementService.AddDocumentRequirement(model);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteDocumentRequirement")]
        public async Task<IActionResult> DeleteDocumentRequirement([FromQuery] int documentRequirementId)
        {
            var result = await _documentRequirementService.DeleteDocumentRequirement(documentRequirementId);
            if (result.Success)
                return Ok();
            return BadRequest();
        }

    }
}
