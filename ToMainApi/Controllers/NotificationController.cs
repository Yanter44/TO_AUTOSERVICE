using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Interfaces;

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
                return Ok(result.Data);

            return BadRequest(result.Message);
        }
    }
}
