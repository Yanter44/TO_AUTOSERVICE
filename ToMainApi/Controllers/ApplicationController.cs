using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using ToMainApi.Features.Application.Events;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Application;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Services;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationservice)
        {
            _applicationService = applicationservice;
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpGet("GetAllApplications")]
        public async Task<IActionResult> GetAllApplications([FromQuery] PaginationDto model)
        {
            var result = await _applicationService.GetAllApplications(model);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Message);
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
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _applicationService.CreateNewApplication(userid, model);
            if (result.Success)
                return Ok();
            
            return BadRequest(result.Message);
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
