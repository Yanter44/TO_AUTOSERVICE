using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToMainApi.Common;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos;
using ToMainApi.Models.Dtos.Auth;
using ToMainApi.Models.Dtos.Invitation;
using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private ILogger<AuthController> _logger;
        private readonly IAuthService _authservice;
        private readonly IUserService _userService;
        private readonly IinvitationService _invitationService;
        public AuthController(IAuthService authservice,
            ILogger<AuthController> logger,
            IUserService userService, IinvitationService invitationService)
        {
            _authservice = authservice;
            _logger = logger;
            _userService = userService;
            _invitationService = invitationService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _authservice.CheckUsers(request);
            if (user == null)
                return BadRequest(new ServiceResponse<bool> { Success = false,
                                                              Message = "Неверный email или пароль" });

            var block = await _authservice.GetActiveBlockAsync(user.Id);
            if (block.IsBlocked)
                return Blocked(block);

            var result = await _authservice.LoginUser(user);
            if (result.Success)
            {
                Response.Cookies.Append("jwt", result.Data.AccessToken, GetCookieOptions(TimeSpan.FromMinutes(30)));
                Response.Cookies.Append("jwtrefresh", result.Data.RefreshToken, GetCookieOptions(TimeSpan.FromDays(30)));
                return Ok(result);
            }
            return BadRequest(result);
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
            if(result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("FinishRegistration")]
        public async Task<IActionResult> FinishRegistration([FromBody] RegistrationDto request)
        {
            var result = await _authservice.FinishRegistration(request);
            if (!result.Success)
                return BadRequest(result);

            Response.Cookies.Append("jwt", result.Data.AccessToken, GetCookieOptions(TimeSpan.FromMinutes(30)));
            Response.Cookies.Append("jwtrefresh", result.Data.RefreshToken, GetCookieOptions(TimeSpan.FromDays(30)));
            return Ok(new ServiceResponse<bool>() { Success = true, Message = "Регистрация успешна"});
        }

        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["jwtrefresh"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new ServiceResponse<bool> { Success = false, Message = "Нет токена" });

            var checkRefreshTokenResult = await _authservice.CheckRefreshToken(refreshToken);
            if (!checkRefreshTokenResult.Success)
                return Unauthorized(checkRefreshTokenResult);

            var block = await _authservice.GetActiveBlockAsync(checkRefreshTokenResult.Data);
            if (block.IsBlocked)
            {
                ClearAuthCookies();
                return Blocked(block);
            }

            var userdtoresult = await _userService.GetUserById(checkRefreshTokenResult.Data);
            if (!userdtoresult.Success || userdtoresult.Data == null)
                return Unauthorized(new ServiceResponse<bool> { Success = false, Message = userdtoresult.Message ?? "Пользователь не найден" });

            var resultJwt = await _authservice.CreateAccessToken(userdtoresult.Data);
            if (!resultJwt.Success || string.IsNullOrEmpty(resultJwt.Data))
                return Unauthorized(new ServiceResponse<bool> { Success = false, Message = resultJwt.Message ?? "Не удалось обновить токен" });

            Response.Cookies.Append("jwt", resultJwt.Data, GetCookieOptions(TimeSpan.FromMinutes(30)));
            return Ok(new ServiceResponse<bool> { Success = true, Message = "Токен обновлён" });
        }

        [HttpPost("AcceptInvitation")]
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationDto model)
        {
            var result = await _authservice.AcceptInvitation(model);
            if (!result.Success)
                return BadRequest(result);

            Response.Cookies.Append("jwt", result.Data.AccessToken, GetCookieOptions(TimeSpan.FromMinutes(30)));
            Response.Cookies.Append("jwtrefresh", result.Data.RefreshToken,GetCookieOptions(TimeSpan.FromDays(10)));
            return Ok(new ServiceResponse<bool>() { Success = true, Message = "Регистрация успешна"});
        }

        [Authorize]
        [HttpGet("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            var refreshToken = Request.Cookies["jwtrefresh"];
            var result = await _authservice.SignOut(refreshToken);
            if (result.Success)
            {
                ClearAuthCookies();
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("Ping")]
        public IActionResult Ping()
        {
            return Ok();
        }

        [Authorize]
        [HttpGet("Me")]
        public IActionResult Me()
        {
            return Ok(new WhoAmIResponseDto
            {
                RoleType = User.FindFirst(ClaimTypes.Role)?.Value,
                FIO = User.FindFirst("FIO")?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
            });
        }

        private IActionResult Blocked(UserBlockInfoDto info)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ServiceResponse<UserBlockInfoDto>
            {
                Success = false,
                Message = "Ваш аккаунт заблокирован",
                Data = info
            });
        }

        private void ClearAuthCookies()
        {
            var cookieOptions = GetCookieOptions(TimeSpan.FromDays(-1));
            Response.Cookies.Append("jwt", "", cookieOptions);
            Response.Cookies.Append("jwtrefresh", "", cookieOptions);
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
