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

        [HttpPost("Som")]
        public async Task<IActionResult> Ggg(IFormFile photo)
        {
            using var stream = photo.OpenReadStream();
            var result = await _metadataEditor.ProcessImage(stream);
            if (result.Success)
                return Ok(result);

            return BadRequest();
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
