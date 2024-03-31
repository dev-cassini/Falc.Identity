using Duende.IdentityServer.EntityFramework.DbContexts;
using Falc.Identity.Server.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Falc.Identity.Server;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = optionsBuilder =>
                {
                    var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres");
                    var migrationsAssemblyName = typeof(Program).Assembly.GetName().Name;
                    
                    optionsBuilder.UseNpgsql(
                        postgresConnectionString, 
                        contextOptionsBuilder => contextOptionsBuilder.MigrationsAssembly(migrationsAssemblyName));
                };
            })
            .AddOperationalStore(options =>
            {
                var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres");
                var migrationsAssemblyName = typeof(Program).Assembly.GetName().Name;
                options.ConfigureDbContext = optionsBuilder =>
                {
                    optionsBuilder.UseNpgsql(
                        postgresConnectionString, 
                        contextOptionsBuilder => contextOptionsBuilder.MigrationsAssembly(migrationsAssemblyName));
                };
            });

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
        app.Services
            .MigrateDatabase<PersistedGrantDbContext>()
            .MigrateDatabase<ConfigurationDbContext>();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseStaticFiles();
        app.UseRouting();
            
        app.UseIdentityServer();
        
        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
