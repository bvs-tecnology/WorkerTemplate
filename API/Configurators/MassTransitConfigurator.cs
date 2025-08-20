using System.Diagnostics.CodeAnalysis;
using API.Consumers;
using Domain.Entities.Dtos;
using MassTransit;

namespace API.Configurators;
[ExcludeFromCodeCoverage]
public static class MassTransitConfigurator
{
    public static IServiceCollection AddLocalMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<TestConsumer>();
            busConfigurator.AddConsumer<TestConsumerDiff>();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["MessageBroker:Host"]!), h =>
                {
                    h.Username(configuration["MessageBroker:Username"]!);
                    h.Password(configuration["MessageBroker:Password"]!);
                });
                cfg.UseInstrumentation();
                cfg.ReceiveEndpoint("test-queue", e =>
                {
                    e.Bind<TestConsumerDto>();
                    e.ConfigureConsumer<TestConsumer>(context);
                });
                cfg.ReceiveEndpoint("test-diff-queue", e =>
                {
                    e.Bind<TestConsumerDto>();
                    e.ConfigureConsumer<TestConsumerDiff>(context);
                });
            });
        });

        return services;
    }
}
