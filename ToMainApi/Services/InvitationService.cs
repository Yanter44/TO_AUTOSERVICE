using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Invitation;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class InvitationService : IinvitationService
    {
        private readonly AppDbContext _dbcontext;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public InvitationService(AppDbContext dbcontext, IEmailService emailService, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _emailService = emailService;
            _configuration = configuration;
            
        }

        public async Task<ServiceResponse<bool>> InviteUser( InviteUserDto model, UserContextDto userContext)
        {
            if (await _dbcontext.Users.AnyAsync(u => u.Email == model.Email))
                return new ServiceResponse<bool> { Success = false, Message = "Пользователь с таким email уже существует" };

            Role role;
            int? branchId = null;
            string inviterLabel;
            if (!Enum.TryParse<Role>(userContext.Role, ignoreCase: true, out var inviterRole))
                return new ServiceResponse<bool> { Success = false, Message = "Роль приглашающего неизвестна" };

            if (inviterRole == Role.Agent)
            {
                var inviter = await _dbcontext.AgentProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == userContext.Id);

                if (inviter is null)
                    return new ServiceResponse<bool> { Success = false, Message = "Профиль агента не найден" };

                if (model.BranchId is null)
                    return new ServiceResponse<bool> { Success = false, Message = "Выберите ветку" };

                var branch = await _dbcontext.AgentBranches
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b =>
                        b.Id == model.BranchId &&
                        b.OwnerAgentId == inviter.Id &&
                        b.IsActive);

                if (branch is null)
                    return new ServiceResponse<bool> { Success = false, Message = "Ветка не найдена или неактивна" };

                role = Role.Agent;
                branchId = branch.Id;
                inviterLabel = "ваш партнёр";
            }
            else if (inviterRole == Role.Admin)
            {
                if (!Enum.TryParse<Role>(model.Role, ignoreCase: true, out role))
                    return new ServiceResponse<bool> { Success = false, Message = $"Неизвестная роль: {model.Role}" };

                branchId = null;
                inviterLabel = "администратор";
            }
            else
            {
                return new ServiceResponse<bool> { Success = false, Message = "Нет прав на приглашение" };
            }

            var token = Guid.NewGuid().ToString("N");

            _dbcontext.UserInvitations.Add(new UserInvitation
            {
                Email = model.Email,
                RoleType = role,
                BranchId = branchId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userContext.Id.Value,
                IsUsed = false,
            });

            await _dbcontext.SaveChangesAsync();

            var frontendUrl = _configuration["App:FrontendUrl"]?.TrimEnd('/')
                ?? throw new InvalidOperationException("App:FrontendUrl не настроен");

            var inviteLink = $"{frontendUrl}/invite?token={token}";

            var roleLabel = role switch
            {
                Role.Agent => "Агента",
                Role.Admin => "Администратора",
                Role.Moderator => "Модератора",
                _ => role.ToString()
            };

            string htmlBody = $@"
                                <div style=""font-family: 'Segoe UI', Arial, sans-serif; margin: 0 auto; max-width: 600px; background-color: #ffffff; border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden;"">
                                    <div style=""background-color: #000000; padding: 20px; text-align: center;"">
                                        <h1 style=""color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 2px;"">ТО-АГЕНТ</h1>
                                    </div>
                                    <div style=""padding: 40px 30px; text-align: center;"">
                                        <div style=""width: 72px; height: 72px; margin: 0 auto 24px; background-color: #eef2ff; border-radius: 50%; line-height: 72px; text-align: center;"">
                                            <span style=""font-size: 36px;"">✉️</span>
                                        </div>
                                        <h2 style=""color: #1f2937; margin: 0 0 12px; font-size: 22px;"">Вас пригласили в команду</h2>
                                        <p style=""color: #6b7280; font-size: 16px; line-height: 1.6; margin: 0 0 28px;"">
                                            Вы получили это письмо, потому что {inviterLabel} пригласил вас на роль
                                            <b style=""color: #4f46e5;"">{roleLabel}</b> в системе <b>ТО-АГЕНТ</b>.
                                        </p>
                                        <div style=""margin: 32px 0;"">
                                            <a href=""{inviteLink}"" style=""display: inline-block; background-color: #4f46e5; color: #ffffff; text-decoration: none; padding: 16px 40px; border-radius: 8px; font-size: 16px; font-weight: 600; letter-spacing: 0.5px;"">
                                                Завершить регистрацию
                                            </a>
                                        </div>
                                        <p style=""color: #9ca3af; font-size: 13px; line-height: 1.5; margin: 24px 0 0;"">
                                            Если кнопка не работает, скопируйте ссылку в браузер:
                                        </p>
                                        <p style=""color: #4f46e5; font-size: 12px; word-break: break-all; margin: 8px 0 0; padding: 10px; background-color: #f9fafb; border-radius: 6px;"">
                                            {inviteLink}
                                        </p>
                                        <p style=""color: #9ca3af; font-size: 13px; line-height: 1.5; margin: 28px 0 0;"">
                                            ⏱ Ссылка действительна в течение <b>7 дней</b>.
                                        </p>
                                    </div>
                                    <div style=""background-color: #f9fafb; padding: 15px; text-align: center; font-size: 12px; color: #9ca3af;"">
                                        © 2026 ТО-АГЕНТ. Все права защищены.
                                    </div>
                                </div>";

            await _emailService.SendEmailAsync(model.Email, "Приглашение в команду TO-Agent!", $"Приглашение в команду TO-Agent! {inviteLink}",htmlBody);
            return new ServiceResponse<bool> { Success = true, Message = "Приглашение успешно отправлено" };
        }
        public async Task<ServiceResponse<InvitationInfoDto>> GetInvitationByToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new ServiceResponse<InvitationInfoDto>
                {
                    Success = false,
                    Message = "Токен не указан"
                };
            }

            var invitation = await _dbcontext.UserInvitations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Token == token);

            if (invitation is null)
            {
                return new ServiceResponse<InvitationInfoDto>
                {
                    Success = false,
                    Message = "Приглашение не найдено"
                };
            }

            if (invitation.IsUsed)
            {
                return new ServiceResponse<InvitationInfoDto>
                {
                    Success = false,
                    Message = "Приглашение уже использовано"
                };
            }

            if (invitation.ExpiresAt < DateTime.UtcNow)
            {
                return new ServiceResponse<InvitationInfoDto>
                {
                    Success = false,
                    Message = "Срок приглашения истёк"
                };
            }

            return new ServiceResponse<InvitationInfoDto>
            {
                Success = true,
                Data = new InvitationInfoDto
                {
                    Email = invitation.Email,
                    RoleType = invitation.RoleType.ToString(),
                    ExpiresAt = invitation.ExpiresAt
                }
            };
        }
        public async Task<ServiceResponse<UserInvitation>> Consume(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return new ServiceResponse<UserInvitation>
                {
                    Success = false,
                    Message = "Токен не указан"
                };

            var invitation = await _dbcontext.UserInvitations
                .FirstOrDefaultAsync(x => x.Token == token);

            if (invitation is null)
                return new ServiceResponse<UserInvitation>
                {
                    Success = false,
                    Message = "Приглашение не найдено"
                };

            if (invitation.IsUsed)
                return new ServiceResponse<UserInvitation>
                {
                    Success = false,
                    Message = "Приглашение уже использовано"
                };

            if (invitation.ExpiresAt < DateTime.UtcNow)
                return new ServiceResponse<UserInvitation>
                {
                    Success = false,
                    Message = "Срок приглашения истёк"
                };

            invitation.IsUsed = true;
            invitation.UsedAt = DateTime.UtcNow;
            await _dbcontext.SaveChangesAsync();

            return new ServiceResponse<UserInvitation>
            {
                Success = true,
                Data = invitation
            };
        }
    }
}
