using Microsoft.EntityFrameworkCore;

namespace Falc.Identity.Server.Data;

public static class ServiceProviderExtensions
{
    public static IServiceProvider MigrateDatabase<T>(this IServiceProvider serviceProvider) 
        where T : DbContext
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<T>();
        dbContext.Database.Migrate();

        return serviceProvider;
    }
}