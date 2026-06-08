using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToMainApi.Models.Dtos;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PromtController : ControllerBase
    {
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("GetAllPromts")]
        public async Task<IActionResult> GetAllPromts()
        {
            return Ok();
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("AddNewPromt")]
        public async Task<IActionResult> AddNewPromt([FromBody] AddNewPromptDto model)
        {
            return Ok();
        }
        [Authorize(Roles ="Admin,Moderator")]
        [HttpDelete("DeletePrompt")]
        public async Task<IActionResult> DeletePrompt ([FromBody] DeletePromptDto model)
        {
            return Ok();
        }

        
    }
}
