using Domain.Entities.Dtos;
using MassTransit;

namespace API.Consumers
{
    public sealed class TestConsumer : IConsumer<UserDto>
    {
        public async Task Consume(ConsumeContext<UserDto> context)
        {
            var message = context.Message;
            await Task.Delay(2000);
            Console.WriteLine($"Received: {message.Id}");
        }
    }
}
