using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Admin;
using ToMainApi.Models.Dtos.Pagination;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IApplicationService _applicationService;
        //TEST_SERVICES
        private readonly IimageMetadataEditor _metadataEditor;
        private readonly IimageTextOverlayService _imagetextoverlayservice;
        public AdminController(IAdminService adminService,
            IApplicationService applicationService, 
            IimageMetadataEditor metadataEditor,
            IimageTextOverlayService imagetextoverlayservice
            )
        {
            _adminService = adminService;
            _applicationService = applicationService;
            _metadataEditor = metadataEditor;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetMyProfile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _adminService.GetMyProfile(userid);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Message);
        }

        [HttpPost("Som")]
        public async Task<IActionResult> Ggg(IFormFile photo)
        {
            using var stream = photo.OpenReadStream();
            var result = await _metadataEditor.ProcessImage(stream);
            if (result.Success)
                return Ok(result.Data);

            return BadRequest();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("ChangeRoleUser")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleDto model)
        {
            var result = await _adminService.ChangeUserRole(model);
            if (result.Success)
                return Ok(result);          
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ChangeUserDebtLimit")]
        public async Task<IActionResult> ChangeUserDebtLimit([FromBody] ChangeUserDebtLimitDto model)
        {
        //    var result = await _adminService.
            return Ok();
        }

        [HttpPost("AddTextToPhoto")]
        public async Task<IActionResult> AddTextToPhoto(IFormFile photo)
        {
            using var stream = photo.OpenReadStream();
            var result = await _imagetextoverlayservice.AddText(stream, "Penis");
            return Ok(result);
        }
    }
}
