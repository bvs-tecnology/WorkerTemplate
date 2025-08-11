using API.Consumers;
using MassTransit;

namespace API.Configurations
{
    public static class RabbitMQConfigurator
    {
        public static IServiceCollection AddLocalRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumer<TestConsumer>();

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(configuration["MessageBroker:Host"]!), h =>
                    {
                        h.Username(configuration["MessageBroker:Username"]!);
                        h.Password(configuration["MessageBroker:Password"]!);
                    });
                    cfg.UseInstrumentation();
                    cfg.ConfigureEndpoints(context);
                    cfg.UseRawJsonSerializer();
                });
            });

            return services;
        }
    }
}
