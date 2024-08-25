using System;
using Microsoft.Extensions.DependencyInjection;
using Falc.Identity.Admin.EntityFramework.Shared.Configuration.Schema;

namespace Falc.Identity.Admin.EntityFramework.Shared.Extensions
{
    public static class ConfigurationSchemaServicesExtensions
    {
        public static IServiceCollection ConfigureAdminAspNetIdentitySchema(this IServiceCollection services,
            Action<IdentityTableConfiguration> configureOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var adminIdentitySchema = new IdentityTableConfiguration();
            configureOptions(adminIdentitySchema);

            services.AddSingleton(adminIdentitySchema);

            return services;
        }
    }
}