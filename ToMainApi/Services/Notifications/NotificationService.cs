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
        private readonly IUserService _userService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(AppDbContext dbcontext, IUserService userService, IHubContext<NotificationHub> hubContext)
        {
            _dbcontext = dbcontext;
            _userService = userService;
            _hubContext = hubContext;
        }
        public async Task<ServiceResponse<NotificationDto>> CreateNewNotification(CreateNotificationRequest model)
        {
            var notification = new Models.Entities.Notification
            {
                Title = model.Title,
                Message = model.Message,
                NotificationType = model.NotificationType,
                CreatedAt = DateTime.UtcNow
            };

            await _dbcontext.Notifications.AddAsync(notification);
            await _dbcontext.SaveChangesAsync();

            var dto = new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                NotificationType = notification.NotificationType,
                CreatedAt = notification.CreatedAt
            };
            return new ServiceResponse<NotificationDto>() { Data = dto, Success = true };
        }

        public async Task NotifyApplicationCreated(int agentId, string agentName, int applicationId)
        {
            var users = await _userService.GetUsersByRoles(new[] { "Admin", "Moderator" });

            var notifications = new List<Notification>();

            foreach (var user in users)
            {
                notifications.Add(new Notification
                {
                    UserId = user.UserId,
                    Title = "Новая заявка",
                    Message = $"Агентом {agentName} была создана заявка №{applicationId}",
                    NotificationType = NotificationEvents.AgentCreateApplication,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }
            var agentNotification = new Notification
            {
                UserId = agentId,
                Title = "Заявка отправлена",
                Message = $"Ваша заявка №{applicationId} отправлена на модерацию.",
                NotificationType = NotificationEvents.ApplicationSendToModeration,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _dbcontext.Notifications.AddRange(notifications);
            _dbcontext.Notifications.Add(agentNotification);
            await _dbcontext.SaveChangesAsync();

            foreach (var notification in notifications)
            {
                await _hubContext.Clients
                    .User(notification.UserId.ToString())
                    .SendAsync(
                        NotificationEvents.AgentCreateApplication,
                        new NotificationDto
                        {
                            Id = notification.Id,
                            Title = notification.Title,
                            Message = notification.Message,
                            NotificationType = notification.NotificationType,
                            CreatedAt = notification.CreatedAt,
                            IsRead = notification.IsRead
                        });
            }
            await _hubContext.Clients
                .User(agentId.ToString())
                .SendAsync(
                    NotificationEvents.ApplicationSendToModeration,
                    new NotificationDto
                    {
                        Title = "Заявка отправлена",
                        Message = $"Ваша заявка №{applicationId} успешно создана и отправлена на модерацию.",
                        NotificationType = NotificationEvents.ApplicationSendToModeration,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    });
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
