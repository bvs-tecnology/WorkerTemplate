using Domain.Entities.Dtos;
using MassTransit;

namespace API.Consumers
{
    public sealed class TestConsumer(ILogger<TestConsumer> logger) : IConsumer<TestConsumerDto>
    {
        public async Task Consume(ConsumeContext<TestConsumerDto> context)
        {
            logger.LogInformation("Consuming TestConsumerDto {id}", context.Message.Id);
            await Task.Delay(500);
            logger.LogInformation("TestConsumerDto {id} Consumed", context.Message.Id);
        }
    }
}
