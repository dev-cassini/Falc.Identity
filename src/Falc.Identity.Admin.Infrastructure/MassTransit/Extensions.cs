using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Falc.Identity.Admin.Infrastructure.MassTransit;

internal static class Extensions
{
    internal static IServiceCollection AddMassTransit(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection
            .AddMassTransit(configurator =>
            {
                configurator.UsingRabbitMq((context, factoryConfigurator) =>
                {
                    var rmqUri = configuration.GetConnectionString("Rmq");
                    factoryConfigurator.Host(rmqUri);
                    factoryConfigurator.ConfigureEndpoints(context, new CustomEndpointNameFormatter());
                });
            });
        
        return serviceCollection;
    }
}