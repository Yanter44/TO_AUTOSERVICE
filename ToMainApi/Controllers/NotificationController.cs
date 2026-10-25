using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpGet("GetNotifications")]
        public async Task<IActionResult> GetNotifications([FromQuery] int page, int pageSize)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _notificationService.GetNotifications(userId, page, pageSize);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize]
        [HttpPatch("MarkNotificationAsRead")]
        public async Task<IActionResult> MarkNotificationAsRead([FromQuery] int notificationId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userContextDto = new UserContextDto() { Id = userId };
            var result = await _notificationService.MarkNotificationAsRead(userContextDto, notificationId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("MarkAllMyNotificationsAsRead")]
        public async Task<IActionResult> MarkAllMyNotificationsAsRead()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userContextDto = new UserContextDto() { Id = userId };
            var result = await _notificationService.MarkAllMyNotificationsAsRead(userContextDto);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
