using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Auth;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IEncryptService _encryptService;
        private readonly IRedisService _redisService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public AuthService(AppDbContext dbcontext,IEncryptService encryptService, 
                           IRedisService redisService, 
                           IEmailService emailservice, IConfiguration configuration)
        {
            _dbContext = dbcontext;
            _encryptService = encryptService;
            _redisService = redisService;
            _emailService = emailservice;
            _configuration = configuration;
        }
        public async Task<User> CheckUsers(LoginDto model)
        {
            if (model.Email != null && model.Password != null)
            {
                var decryptedPassword = _encryptService.Encrypt(model.Password);
                var result = await _dbContext.Users.Where(x => x.Email == model.Email && x.Password == decryptedPassword)
                                      .FirstOrDefaultAsync();
                if(result != null)
                { 
                    return result;
                }
            }
            return null;
        }
        public Task<ServiceResponse<string>> LoginUser(User usermodel)
        {
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Email, usermodel.Email),
               new Claim(ClaimTypes.Role, usermodel.RoleType)
            };
            var secretkey = _configuration["Jwt:Key"];
            var secretissuer = _configuration["Jwt:Issuer"];
            var secretaudience = _configuration["Jwt:Audience"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: secretissuer,      
                audience: secretaudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(new ServiceResponse<string>() { Data = jwt, Success = true});
        }
        public async Task<ServiceResponse<bool>> ConfirmRegistrationCode(ConfirmCodeDto model)
        {
            var code = await _redisService.GetStringAsync(model.RegistrationId);
            if (code != null && code == model.Code)
            {
                await _redisService.DeleteAsync(model.RegistrationId);
                await _redisService.SetStringAsync($"{model.RegistrationId}_CodeConfirmed", "true", TimeSpan.FromDays(1));
                return new ServiceResponse<bool> { Success = true };
            }
            return new ServiceResponse<bool> { Success = false };
        }
        public async Task<ServiceResponse<string>> TryRegistration(TryRegistrationDto model)
        {
            var existuser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (existuser == null)
            {
                var registrationId = Guid.NewGuid().ToString();
                var codeforuser = CodeGeneratorService.GenerateSixDigitCode();
                await _redisService.SetStringAsync(registrationId, codeforuser, TimeSpan.FromMinutes(5));

                string htmlBody = $@"
                                <div style=""font-family: 'Segoe UI', Arial, sans-serif; margin: 0 auto; background-color: #ffffff; border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden;"">
                                    <div style=""background-color: #000000; padding: 20px; text-align: center;"">
                                        <h1 style=""color: #ffffff; margin: 0; font-size: 24px;"">ТО-АГЕНТ</h1>
                                    </div>
                                    <div style=""padding: 30px; text-align: center;"">
                                        <h2 style=""color: #1f2937; margin-bottom: 10px;"">Подтверждение входа</h2>
                                        <p style=""color: #6b7280; font-size: 16px; line-height: 1.5; margin-bottom: 30px;"">
                                            Здравствуйте! Кто-то запросил код для входа в ваш аккаунт. Если это были вы, введите код ниже:
                                        </p>
                                        <div style=""display: inline-block; background-color: #f3f4f6; padding: 15px 30px; border-radius: 8px; border: 2px dashed #4f46e5;"">
                                            <span style=""font-size: 32px; font-weight: bold; color: #111827; letter-spacing: 8px;"">{codeforuser}</span>
                                        </div>
                                        <p style=""color: #9ca3af; font-size: 14px; margin-top: 30px;"">
                                            Код истекает через 5 минут. Если вы не запрашивали код, просто проигнорируйте это письмо.
                                        </p>
                                    </div>
                                    <div style=""background-color: #f9fafb; padding: 15px; text-align: center; font-size: 12px; color: #9ca3af;"">
                                        © 2026 ТО-АГЕНТ. Все права защищены.
                                    </div>
                                </div>";
                await _emailService.SendEmailAsync(model.Email, "Код подтверждения", $"Ваш код подтверждения: {codeforuser}", htmlBody);
                return new ServiceResponse<string> { Data = registrationId, Success = true };
            }
            return new ServiceResponse<string> { Success = false, Message = "Пользователь с таким email зарегистрирован." +
                                                                            "Войдите в аккаунт или используйте другой адрес" };
        }
        public async Task<ServiceResponse<string>> FinishRegistration(RegistrationDto registermodel)
        {
            var isConfirmed = await _redisService.GetStringAsync($"{registermodel.RegistrationId}_CodeConfirmed");

            if (string.IsNullOrEmpty(isConfirmed))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Почта не была подтверждена или время сессии истекло. Пожалуйста, начните регистрацию заново."
                };
            }
            var newUser = new User()
            {
                Email = registermodel.Email,
                FIO = registermodel.FIO,
                Password = _encryptService.Encrypt(registermodel.Password),
                RoleType = Role.Agent.ToString()
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
            await _redisService.DeleteAsync($"{registermodel.RegistrationId}_CodeConfirmed");
            var result = await LoginUser(newUser);
            return new ServiceResponse<string> { Data = result.Data, Success = true };
        }
    }
}
