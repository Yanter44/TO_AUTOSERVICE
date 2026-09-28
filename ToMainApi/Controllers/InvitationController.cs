using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Common;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Invitation;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Services;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvitationController : ControllerBase
    {
        private readonly IinvitationService _invitationService;
        public InvitationController(IinvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [Authorize(Roles = "Admin,Agent")]
        [HttpPost("InviteUser")]
        public async Task<IActionResult> InviteUser([FromBody] InviteUserDto model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = (User.FindFirst(ClaimTypes.Role)?.Value);
            var userContextModel = new UserContextDto() { Id = userId, Role = userRole };
            var result = await _invitationService.InviteUser(model, userContextModel);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("GetInvitationByToken")]
        public async Task<IActionResult> GetInvitationByToken([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new ServiceResponse<bool>() { Success = false, Message = "Токен не найден" });

            var result = await _invitationService.GetInvitationByToken(token);

            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
