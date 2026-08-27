using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Notification;
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
    }
}
