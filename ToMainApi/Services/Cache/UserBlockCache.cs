using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Services.Cache
{
    public class UserBlockCache : IUserBlockCache
    {
        private readonly IRedisService _redis;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<UserBlockCache> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

        public UserBlockCache(
            IRedisService redis,
            IServiceScopeFactory scopeFactory,
            ILogger<UserBlockCache> logger)
        {
            _redis = redis;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<UserBlockInfoDto> GetBlockInfoAsync(int userId)
        {
            var key = $"user_block:{userId}";

            // 1. Пробуем Redis
            try
            {
                var cached = await _redis.GetObjectAsync<UserBlockInfoDto>(key);
                if (cached != null) return cached;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis недоступен, читаем из БД");
            }

            // 2. Идём в БД
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var status = await db.UserStatuses
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Select(s => new { s.IsBlocked, s.BlockedUntil })
                .FirstOrDefaultAsync();

            var isBlocked = status != null
                && status.IsBlocked
                && (status.BlockedUntil == null || status.BlockedUntil > DateTime.UtcNow);

            string? reason = null;

            if (isBlocked)
            {
                reason = await db.UserBlocks
                    .AsNoTracking()
                    .Where(b => b.UserId == userId && b.UnblockedAt == null)
                    .OrderByDescending(b => b.BlockedAt)
                    .Select(b => b.Reason)
                    .FirstOrDefaultAsync();
            }

            var info = new UserBlockInfoDto
            {
                IsBlocked = isBlocked,
                Reason = reason,
                BlockedUntil = isBlocked ? status!.BlockedUntil : null
            };
            try
            {
                await _redis.SetObjectAsync(key, info, Ttl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не записалось в Redis");
            }
            return info;
        }

        public async Task InvalidateAsync(int userId)
        {
            try
            {
                await _redis.DeleteAsync($"user_blocked:{userId}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не инвалидировалось для {UserId}", userId);
            }
        }
    }
}
