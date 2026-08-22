using Microsoft.EntityFrameworkCore;
using ToMainApi.DbContext;

namespace ToMainApi.Services.BackgorundServices
{
    public class RefreshTokenCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RefreshTokenCleanupService> _logger;

        public RefreshTokenCleanupService(IServiceScopeFactory scopeFactory,
            ILogger<RefreshTokenCleanupService> logger)
        {
            _serviceScopeFactory = scopeFactory;
            _logger = logger;   
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromHours(1));
                    using var scope = _serviceScopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var now = DateTime.UtcNow;
                    var expiredTokens = await db.RefreshTokens.Where(x => x.ExpiresAt < now).ToListAsync();
                    if (expiredTokens.Any())
                    {
                        db.RefreshTokens.RemoveRange(expiredTokens);
                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"Удалено {expiredTokens.Count} просроченных refresh токенов");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при очистке refresh токенов :((");
                }
            }
        }
    }
}
