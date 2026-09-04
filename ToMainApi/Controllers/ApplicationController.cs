using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Common;
using ToMainApi.Interfaces;
using ToMainApi.Models;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Application;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICloudinaryService _cloudinaryService;
        public ApplicationController(IApplicationService applicationservice, 
            IServiceScopeFactory scopefactory, ICloudinaryService cloudinaryService)
        {
            _applicationService = applicationservice;
            _scopeFactory = scopefactory;
            _cloudinaryService = cloudinaryService;
        }
        [Authorize]
        [HttpGet("GetApplications")]
        public async Task<IActionResult> GetApplications([FromQuery] PaginationDto model)
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userrole = (User.FindFirst(ClaimTypes.Role)?.Value);
            var usercontext = new UserContextDto() { Id = userid, Role = userrole };
            var result = await _applicationService.GetApplications(usercontext, model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("GetApplicationsMetrics")]
        public async Task<IActionResult> GetApplicationsMetrics()
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userrole = (User.FindFirst(ClaimTypes.Role)?.Value);
            var userContext = new UserContextDto() { Id = userid, Role = userrole};
            var result = await _applicationService.GetApplicationsMetrics(userContext);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);

        }

        [Authorize(Roles ="Admin,Moderator")]
        [HttpDelete("DeleteApplication")]
        public async Task<IActionResult> DeleteApplication([FromBody] DeleteApplicationDto model)
        {
            var result = await _applicationService.DeleteApplication(model);
            if (result.Success)
                return Ok();
            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Agent")]
        [HttpPost("CreateNewApplication")]
        public async Task<IActionResult> CreateNewApplication([FromForm][Bind(Prefix = "")] CreateNewApplicationDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ServiceResponse<bool>() { Success = false, Message = $"Ошибка валидации [{errors}]" });
            }

            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var applicationService = scope.ServiceProvider.GetRequiredService<IApplicationService>();
                var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                 await applicationService.CreateNewApplication(userid, model);
            });

            return Ok(new ServiceResponse<bool>()
            {
                Success = true,
                Message = "Заявка успешно отправлена на обработку"
            });
        }

        [Authorize(Roles ="Admin,Moderator")]
        [HttpPost("ConfirmAndSendApplication")]
        public async Task<IActionResult> ConfirmAndSendApplication()
        {
            return Ok();
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpPost("RejectApplication")]
        public async Task<IActionResult> RejectApplication()
        {
            return Ok();
        }
    }
}
