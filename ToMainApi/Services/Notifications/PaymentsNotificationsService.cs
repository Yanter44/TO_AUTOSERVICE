using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Notification;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services.Notifications
{
    public class PaymentsNotificationsService : IPaymentNotificationService
    {
        private readonly INotificationsSender _notificationSender;
        private readonly IUserService _userService;
        private readonly AppDbContext _dbcontext;
        public PaymentsNotificationsService(INotificationsSender notificationSender,
            IUserService userService, AppDbContext dbcontext)
        {
            _notificationSender = notificationSender;
            _userService = userService;
            _dbcontext = dbcontext;
        }
        public async Task NotifyBalanceCredited(int agentId, decimal ammount)
        {
            var agentNotification = new Notification
            {
                UserId = agentId,
                Title = "Поступление средств",
                Message = $"На ваш баланс зачислено {ammount}₽",
                NotificationType = NotificationEvents.BalanceCredited,
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
            await _notificationSender.SendToUser(agentId, NotificationEvents.BalanceCredited, agentDto);
        }
        public async Task NotifyBalanceDebited(int agentId, decimal ammount)
        {
            var agentNotification = new Notification
            {
                UserId = agentId,
                Title = "Списание средств",
                Message = $"С вашего баланса списано {ammount}₽",
                NotificationType = NotificationEvents.BalanceDebited,
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
            await _notificationSender.SendToUser(agentId, NotificationEvents.BalanceDebited, agentDto);
        }
    }
}
