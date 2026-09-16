using System.Security.Claims;
using ToMainApi.Common;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Middlewares
{
    public class BlockedUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BlockedUserMiddleware> _logger;

        public BlockedUserMiddleware(RequestDelegate next, ILogger<BlockedUserMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context, IUserBlockCache cache)
        {
            //  Анонимные запросы пропускаем
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                await _next(context);
                return;
            }
            try
            {
                var info = await cache.GetBlockInfoAsync(userId);

                if (info.IsBlocked)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new ServiceResponse<UserBlockInfoDto>()
                    {
                        Data = new UserBlockInfoDto() { IsBlocked = true, BlockedUntil = info.BlockedUntil, Reason = info.Reason},
                        Success = false,
                        Message = "Ваш аккаунт заблокирован!"
                    });
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка проверки блокировки для {UserId}", userId);
            }

            await _next(context);
        }
    }
}
