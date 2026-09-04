using MediatR;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Notification;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services.Notifications
{
    public class ApplicationNotificationsService : IApplicationNotificationService
    {
        private readonly INotificationsSender _notificationSender;
        private readonly IUserService _userService;
        private readonly AppDbContext _dbcontext;
        private readonly ILogger<ApplicationNotificationsService> _logger;
        public ApplicationNotificationsService(INotificationsSender notificationSender, 
            IUserService userService, AppDbContext dbcontext, ILogger<ApplicationNotificationsService> logger)
        {
            _notificationSender = notificationSender;
            _userService = userService;
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task NotifyApplicationProcessingError(int applicationId, int agentId, string errorMessage)
        {
            try
            {
                _logger.LogError(
                    "❌ Ошибка обработки заявки. UserId: {UserId}, ApplicationId: {ApplicationId}, Error: {ErrorMessage}",
                    agentId,
                    applicationId,
                    errorMessage
                );

                var agent = await _userService.GetAgentByUserId(agentId);
                if (agent == null || !agent.Success)
                {
                    _logger.LogWarning("⚠️ Пользователь {UserId} не найден", agentId);
                    return;
                }

                string title;
                string message;

                if (applicationId == 0)
                {
                    title = "Ошибка создания заявки";
                    message = $"Не удалось создать заявку. Ошибка: {errorMessage}";
                }
                else
                {
                    title = "Ошибка обработки заявки";
                    message = $"Заявка №{applicationId} не прошла обработку. Ошибка: {errorMessage}";
                }

                var agentNotification = new Notification
                {
                    UserId = agentId,
                    Title = title,
                    Message = message,
                    NotificationType = NotificationEvents.ApplicationProcessingError,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _dbcontext.Notifications.AddAsync(agentNotification);
                await _dbcontext.SaveChangesAsync();

                var agentDto = new NotificationDto
                {
                    Id = agentNotification.Id,
                    Title = agentNotification.Title,
                    Message = agentNotification.Message,
                    NotificationType = agentNotification.NotificationType,
                    CreatedAt = agentNotification.CreatedAt,
                    IsRead = agentNotification.IsRead
                };
                await _notificationSender.SendToUser(agentId, NotificationEvents.ApplicationProcessingError, agentDto);
                _logger.LogInformation("✅ Уведомление об ошибке отправлено пользователю {UserId}", agentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Критическая ошибка при отправке уведомления об ошибке");
            }
        }

        public async Task NotifyApplicationCreated(int applicationId, int agentId)
        {
            var agent = await _userService.GetAgentByUserId(agentId);
            if (agent == null || !agent.Success) return;

            var adminsAndModerators = await _userService.GetUsersByRoles(new[] { "Admin", "Moderator" });
            var adminModeratorIds = adminsAndModerators.Select(u => u.UserId).ToList();

            var adminModeratorNotifications = adminModeratorIds.Select(userId => new Notification
            {
                UserId = userId,
                Title = "Новая заявка",
                Message = $"Агент {agent.Data.FIO} отправил на рассмотрение заявку №{applicationId}",
                NotificationType = NotificationEvents.ApplicationCreated,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            }).ToList();

            await _dbcontext.Notifications.AddRangeAsync(adminModeratorNotifications);
            await _dbcontext.SaveChangesAsync();

            var agentNotification = new Notification
            {
                UserId = agentId,
                Title = "Заявка отправлена",
                Message = $"Ваша заявка №{applicationId} отправлена на модерацию.",
                NotificationType = NotificationEvents.ApplicationSendToModeration,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _dbcontext.Notifications.AddAsync(agentNotification);
            await _dbcontext.SaveChangesAsync();

            var adminModeratorDtos = adminModeratorNotifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                NotificationType = n.NotificationType,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToList();

            var agentDto = new NotificationDto
            {
                Id = agentNotification.Id,
                Title = agentNotification.Title,
                Message = agentNotification.Message,
                NotificationType = agentNotification.NotificationType,
                CreatedAt = agentNotification.CreatedAt,
                IsRead = agentNotification.IsRead
            };

            await _notificationSender.SendToUsers(adminModeratorIds, NotificationEvents.ApplicationCreated, adminModeratorDtos);
            await _notificationSender.SendToUser(agentId, NotificationEvents.ApplicationSendToModeration, agentDto);
        }

        public async Task NotifyApplicationApproved(int applicationId, int agentId)
        {
            var agent = await _userService.GetAgentByUserId(agentId);
            if (agent == null || !agent.Success) return;

            var agentNotification = new Notification
            {
                UserId = agentId,
                Title = "Заявка прошла модерацию",
                Message = $"Заявка №{applicationId} успешно прошла модерацию!",
                NotificationType = NotificationEvents.ApplicationRejected,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _dbcontext.Notifications.AddAsync(agentNotification);
            await _dbcontext.SaveChangesAsync();
            var agentDto = new NotificationDto()
            {
                Id = agentNotification.Id,
                Title = agentNotification.Title,
                Message = agentNotification.Message,
                NotificationType = agentNotification.NotificationType,
                CreatedAt = agentNotification.CreatedAt,
                IsRead = agentNotification.IsRead
            };
            await _notificationSender.SendToUser(agentId, NotificationEvents.ApplicationRejected, agentDto);
        }
        public async Task NotifyApplicationRejected(int applicationId, int agentId)
        {
            var agent = await _userService.GetAgentByUserId(agentId);
            if (agent == null || !agent.Success) return;

            var agentNotification = new Notification
            {
                UserId = agentId,
                Title = "Заявка отклонена",
                Message = $"Заявка №{applicationId} не прошла модерацию.",
                NotificationType = NotificationEvents.ApplicationRejected,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _dbcontext.Notifications.AddAsync(agentNotification);
            await _dbcontext.SaveChangesAsync();
            var agentDto = new NotificationDto()
            {
                Id = agentNotification.Id,
                Title = agentNotification.Title,
                Message = agentNotification.Message,
                NotificationType = agentNotification.NotificationType,
                CreatedAt = agentNotification.CreatedAt,
                IsRead = agentNotification.IsRead
            };
            await _notificationSender.SendToUser(agentId, NotificationEvents.ApplicationRejected, agentDto);
        }
    }
}
