using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Notification;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _dbcontext;

        public NotificationService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;

        }
        public async Task<ServiceResponse<List<NotificationDto>>> GetNotifications(int userId,int page, int pageSize)
        {
            var notifications = await _dbcontext.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    NotificationType = n.NotificationType,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                })
                .ToListAsync();

            return new ServiceResponse<List<NotificationDto>>
            {
                Success = true,
                Data = notifications
            };
        }
        public async Task<ServiceResponse<bool>> MarkNotificationAsRead( UserContextDto userContext, int notificationId)
        {
            if (userContext.Id is null or <= 0)
                return new ServiceResponse<bool> { Success = false, Message = "Пользователь не авторизован" };

            if (notificationId <= 0)
                return new ServiceResponse<bool> { Success = false, Message = "Некорректный Id уведомления" };

            var notification = await _dbcontext.Notifications
                .FirstOrDefaultAsync(
                    x => x.Id == notificationId && x.UserId == userContext.Id);

            if (notification is null)
                return new ServiceResponse<bool> { Success = false, Message = "Уведомление не найдено" };

            notification.IsRead = true;
            await _dbcontext.SaveChangesAsync();
            return new ServiceResponse<bool> { Success = true, Message = "Уведомление помечено как прочитанное" };
        }
        public async Task<ServiceResponse<bool>> MarkAllMyNotificationsAsRead(UserContextDto userContext)
        {
            if (userContext.Id is null or <= 0)
                return new ServiceResponse<bool> { Success = false, Message = "Пользователь не авторизован" };

            await _dbcontext.Notifications
                .Where(x => x.UserId == userContext.Id.Value && !x.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRead, true));

            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Все уведомления помечены как прочитанные"
            };
        }
    }
}
