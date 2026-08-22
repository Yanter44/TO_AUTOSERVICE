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
        private readonly INotificationSender _notificationSender;
        public AdminController(IAdminService adminService,
            IApplicationService applicationService, 
            IimageMetadataEditor metadataEditor,
            IimageTextOverlayService imagetextoverlayservice,
            INotificationSender notificationSender)
        {
            _adminService = adminService;
            _applicationService = applicationService;
            _metadataEditor = metadataEditor;
            _imagetextoverlayservice = imagetextoverlayservice;
            _notificationSender = notificationSender;
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
        public async Task<IActionResult> ChangeUserRole()
        {
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("ChangeUserDebtLimit")]
        public async Task<IActionResult> ChangeUserDebtLimit([FromBody] ChangeUserDebtLimitDto model)
        {
        //    var result = await _adminService.
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("ChangeUserStatus")]
        public async Task<IActionResult> ChangeUserStatus()
        {
            return Ok();
        }

        [HttpPost("AddTextToPhoto")]
        public async Task<IActionResult> AddTextToPhoto(IFormFile photo)
        {
            using var stream = photo.OpenReadStream();
            var result = await _imagetextoverlayservice.AddText(stream, "Penis");
            return Ok(result);
        }

        [HttpPost("SendSomethingToAgents")]
        public async Task<IActionResult> SendSomethingToAgents()
        {
            _notificationSender.SomethingToAgents("sperma");
            return Ok();

        }
    }
}
