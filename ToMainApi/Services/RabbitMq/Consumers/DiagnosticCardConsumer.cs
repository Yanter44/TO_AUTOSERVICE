using MassTransit;
using Shared.ApplicationContracts;

namespace ToMainApi.Services.RabbitMq.Consumers
{
    public class DiagnosticCardConsumer : IConsumer<DiagnosticCardReady>
    {
        private readonly ILogger<DiagnosticCardConsumer> _logger;

        public DiagnosticCardConsumer(ILogger<DiagnosticCardConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<DiagnosticCardReady> context)
        {
            // Тут мы просто пишем в лог, что карта получена
            _logger.LogInformation("!!! УРА! CRM ПОЛУЧИЛА КАРТУ для заявки {Id}. Содержимое карты: {Card}");

            // В реальном проекте тут будет код обновления статуса в БД (через EF Core)
            // Например: _db.Applications.Update(...)

            return Task.CompletedTask;
        }
    }
}
