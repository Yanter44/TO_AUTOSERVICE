using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Ai;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {          
            var result = await _userService.GetAllUsers();
            if(result.Success)
                return Ok(result.Data);

            return BadRequest(result.Message);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsersExcept")]
        public async Task<IActionResult> GetAllUsersExcept()
        {
            var currentuserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userService.GetAllUsersExcept(int.Parse(currentuserId));
            if (result.Success)
                return Ok(result.Data);

            return BadRequest(result.Message);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllAgents")]
        public async Task<IActionResult> GetAllAgents()
        {
            var result = await _userService.GetAllAgents();
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("GetAvailableRoles")]
        public IActionResult GetAvailableRoles()
        {
            var roles = Enum.GetValues<Role>()
                .Select(x => new UserRoleDto { Id = x, Name = x.ToString() }).ToList();
            return Ok(roles);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] int userId)
        {

            return Ok();
        }
    }
}
