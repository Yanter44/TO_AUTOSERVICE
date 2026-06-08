using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos;
using ToMainApi.Models.Enums;
using ToMainApi.Services;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PromtController : ControllerBase
    {
        private readonly IPromptService _promtService;
        private readonly IModeratorService _moderatorService;
        public PromtController(IPromptService promptService, 
                               IModeratorService moderatorService)
        {
            _promtService = promptService;
            _moderatorService = moderatorService;
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("GetAllPromts")]
        public async Task<IActionResult> GetAllPromts()
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _promtService.GetAllPrompts(userid);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("AddNewPromt")]
        public async Task<IActionResult> AddNewPromt([FromBody] AddNewPromptDto model)
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _promtService.AddNewPromptAsync(userid, model);
            if (result.Success)
                return Ok(result.Data);

            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpDelete("DeletePrompt")]
        public async Task<IActionResult> DeletePrompt([FromBody] DeletePromptDto model)
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _promtService.DeletePromptAsync(userid, model);
            if (result.Success)
                return Ok(result.Data);

            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("UpdatePrompt")]
        public async Task<IActionResult> UpdatePrompt([FromBody] UpdatePromptDto model)
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _promtService.UpdatePromptAsync(userid, model);
            if (result.Success)
                return Ok(result.Data);

            return BadRequest(result.Message);
        }


    }
}
