using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Admin;
using ToMainApi.Models.Dtos.Ai;
using ToMainApi.Models.Dtos.Pagination;
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

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {          
            var result = await _userService.GetAllUsers();
            if(result.Success)
                return Ok(result.Data);

            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationDto model)
        {
            var result = await _userService.GetUsers(model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
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
                return Ok(result);
            return BadRequest(result);
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
        [HttpPost("BlockUser")]
        public async Task<IActionResult> BlockUser([FromBody] BlockUserDto model)
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userrole = (User.FindFirst(ClaimTypes.Role)?.Value);
            var usercontext = new UserContextDto() { Id = userid, Role = userrole };
            var result = await _userService.BlockUser(usercontext, model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUserBlocksHistory")]
        public async Task<IActionResult> GetUserBlocksHistory([FromQuery] int userId)
        {
            var result = await _userService.GetUserBlockHistory(userId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("UnblockUser")]
        public async Task<IActionResult> UnblockUser([FromBody] UnblockUserDto model)
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userrole = User.FindFirst(ClaimTypes.Role)?.Value;

            var usercontext = new UserContextDto() { Id = userid, Role = userrole };

            var result = await _userService.UnblockUser(usercontext, model);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ChangeUserDebtLimit")]
        public async Task<IActionResult> ChangeUserDebtLimit([FromBody] ChangeUserDebtLimitDto model)
        {
            var result = await _userService.ChangeUserDebtLimit(model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
