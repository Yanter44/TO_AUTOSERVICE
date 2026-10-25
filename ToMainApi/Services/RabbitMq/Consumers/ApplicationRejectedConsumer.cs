using MassTransit;
using Shared.ApplicationContracts;

namespace ToMainApi.Services.RabbitMq.Consumers
{
    public class ApplicationRejectedConsumer : IConsumer<ApplicationRejected>
    {
        public Task Consume(ConsumeContext<ApplicationRejected> context)
        {
            throw new NotImplementedException();
        }
    }
}
