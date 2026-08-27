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
        private ILogger<AuthController> _logger;
        private readonly IAuthService _authservice;
        private readonly IUserService _userService;
        public AuthController(IAuthService authservice,
            ILogger<AuthController> logger,
            IUserService userService)
        {
            _authservice = authservice;
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email и пароль обязательны" });
            }

            var user = await _authservice.CheckUsers(request);
            if (user == null)
            {
                return BadRequest(new { message = "Неверный email или пароль" });
            }

            var result = await _authservice.LoginUser(user);
            if (result.Success)
            {
                Response.Cookies.Append("jwt", result.Data.AccessToken, GetCookieOptions(TimeSpan.FromMinutes(1)));
                Response.Cookies.Append("jwtrefresh", result.Data.RefreshToken, GetCookieOptions(TimeSpan.FromDays(30)));

                return Ok(new { message = "Успешный вход" });
            }
            return BadRequest(new { message = result.Message ?? "Ошибка при входе" });
        }
        [HttpPost("TryRegistration")]
        public async Task<IActionResult> TryRegistration([FromBody] TryRegistrationDto request)
        {
            var result = await _authservice.TryRegistration(request);
            if (result.Success)
                 return Ok(result);
            return BadRequest(result);
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

            Response.Cookies.Append("jwt", result.Data.AccessToken, GetCookieOptions(TimeSpan.FromMinutes(50)));
            Response.Cookies.Append("jwtrefresh", result.Data.RefreshToken, GetCookieOptions(TimeSpan.FromDays(30)));
            return Ok(new { message = "Регистрация успешна" });
        }

        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["jwtrefresh"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var checkRefreshTokenResult = await _authservice.CheckRefreshToken(refreshToken);
            if (checkRefreshTokenResult.Success)
            {
                var userdtoresult = await _userService.GetUserById(checkRefreshTokenResult.Data);
                var resultJwt = await _authservice.CreateAccessToken(userdtoresult.Data);
                Response.Cookies.Append("jwt", resultJwt.Data, GetCookieOptions(TimeSpan.FromMinutes(50)));
                return Ok();
            }
            return Unauthorized();         
        }

        [Authorize]
        [HttpGet("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            var refreshToken = Request.Cookies["jwtrefresh"];
            var result = await _authservice.SignOut(refreshToken);
            if (result.Success)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(-1)
                };
                Response.Cookies.Append("jwt", "", cookieOptions);
                Response.Cookies.Append("jwtrefresh", "", cookieOptions);
                return Ok();
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet("Ping")]
        public IActionResult Ping()
        {
            return Ok();
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
                Expires = DateTimeOffset.UtcNow.Add(expiration)
            };
        }
    }
}
