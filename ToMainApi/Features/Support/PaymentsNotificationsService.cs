using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Notification;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Features.Support
{
    public class PaymentsNotificationsService : IPaymentNotificationService
    {
        private readonly INotificationsSender _notificationSender;
        private readonly AppDbContext _dbcontext;
        public PaymentsNotificationsService(INotificationsSender notificationSender,AppDbContext dbcontext)
        {
            _notificationSender = notificationSender;
            _dbcontext = dbcontext;
        }
        public async Task NotifyBalanceCredited(int userId, decimal ammount)
        {
            var agentNotification = new Notification
            {
                UserId = userId,
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
            await _notificationSender.SendToUser(userId, NotificationEvents.BalanceCredited, agentDto);
        }
        public async Task NotifyBalanceDebited(int userId, decimal ammount)
        {
            var agentNotification = new Notification
            {
                UserId = userId,
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
            await _notificationSender.SendToUser(userId, NotificationEvents.BalanceDebited, agentDto);
        }
    }
}
