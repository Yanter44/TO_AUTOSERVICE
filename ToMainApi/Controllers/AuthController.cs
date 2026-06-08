using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos;
using ToMainApi.Models.Dtos.Auth;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authservice;
        public AuthController(IAuthService authservice)
        {
            _authservice = authservice;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _authservice.CheckUsers(request);
            if(user == null)
            {
                return BadRequest();
            }
            var result = await _authservice.LoginUser(user);
            if (result.Success)
            {
                var cookieoption = GetCookieOptions(TimeSpan.FromMinutes(15));
                Response.Cookies.Append("jwt", result.Data, cookieoption);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("TryRegistration")]
        public async Task<IActionResult> TryRegistration([FromBody] TryRegistrationDto request)
        {
            var result = await _authservice.TryRegistration(request);
            return Ok(result);
        }
        [HttpPost("ConfirmRegistrationCode")]
        public async Task<IActionResult> ConfirmRegistrationCode([FromBody] ConfirmCodeDto model)
        {
            var result = await _authservice.ConfirmRegistrationCode(model);
            if(result.Success == true)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost("FinishRegistration")]
        public async Task<IActionResult> FinishRegistration([FromBody] RegistrationDto request)
        {
            var result = await _authservice.FinishRegistration(request);
            if (!result.Success)
                return BadRequest(result.Message);

            Response.Cookies.Append("jwt", result.Data, GetCookieOptions(TimeSpan.FromMinutes(15)));
            return Ok(new { message = "Регистрация успешна" });
        }

        [Authorize]
        [HttpGet("Me")]
        public async Task<IActionResult> WhoAmI()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new WhoAmIResponseDto { RoleType = role });
        }
        private CookieOptions GetCookieOptions(TimeSpan expiration)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            };
        }
    }
}
