using Domain.Entities.Dtos;
using MassTransit;

namespace API.Consumers;

public class TestConsumerDiff(ILogger<TestConsumerDiff> logger) : IConsumer<TestConsumerDto>
{
    public async Task Consume(ConsumeContext<TestConsumerDto> context)
    {
        logger.LogInformation("ConsumingDiff TestConsumerDto {Id}", context.Message.Id);
        await Task.Delay(1000);
        logger.LogInformation("TestConsumerDto {Id} ConsumedDiff", context.Message.Id);
    }
}