using ToMainApi.DbContext;

namespace ToMainApi.Services.BackgorundServices
{
    public class EaistoWorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EaistoWorkerService> _logger;
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(15);

        public EaistoWorkerService(IServiceScopeFactory serviceScopeFactory, ILogger<EaistoWorkerService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                   //using var scope = _serviceScopeFactory.CreateScope();
                   //var dbcontext =  scope.ServiceProvider.GetService<AppDbContext>();
                   //var thread = new Thread(spermo) { Name = "sperma" };

                   // thread.Start();
                }
                catch (Exception ex) { }

            }
            throw new Exception();
        }

    }
}
