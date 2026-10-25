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
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Agent")]
        [HttpPost("CreateNewApplication")]
        public async Task<IActionResult> CreateNewApplication([FromBody] CreateNewApplicationDto model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userContext = new UserContextDto() { Id = userId };
            var result = await _applicationService.CreateNewApplication(userContext, model);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles ="Admin,Moderator")]
        [HttpPost("AcceptApplication")]
        public async Task<IActionResult> AcceptApplication([FromBody] AcceptApplicationDto model)
        {
            var result = await _applicationService.AcceptApplication(model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles ="Admin,Moderator")]
        [HttpPost("RejectApplication")]
        public async Task<IActionResult> RejectApplication([FromBody] RejectApplicationDto model)
        {
            var result = await _applicationService.RejectApplication(model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
