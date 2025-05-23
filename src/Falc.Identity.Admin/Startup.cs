// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Falc.Identity.Admin.Helpers;
using Falc.Identity.Admin.Infrastructure;
using Falc.Identity.Admin.Infrastructure.EntityFramework.DbContexts;
using Falc.Identity.Admin.Infrastructure.EntityFramework.Entities.Identity;
using Skoruba.Duende.IdentityServer.Admin.UI.Helpers.ApplicationBuilder;
using Skoruba.Duende.IdentityServer.Admin.UI.Helpers.DependencyInjection;
using Skoruba.Duende.IdentityServer.Shared.Configuration.Helpers;
using Falc.Identity.Shared.Dtos;
using Falc.Identity.Shared.Dtos.Identity;

namespace Falc.Identity.Admin;
    
public class Startup
{
    public Startup(IWebHostEnvironment env, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        HostingEnvironment = env;
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public IWebHostEnvironment HostingEnvironment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Adds the Duende IdentityServer Admin UI with custom options.
        services
            .AddIdentityServerAdminUI<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext,
            AdminLogDbContext, AdminAuditLogDbContext, AuditLog, IdentityServerDataProtectionDbContext,
            UserIdentity, UserIdentityRole, UserIdentityUserClaim, UserIdentityUserRole,
            UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken, string,
            IdentityUserDto, IdentityRoleDto, IdentityUsersDto, IdentityRolesDto, IdentityUserRolesDto,
            IdentityUserClaimsDto, IdentityUserProviderDto, IdentityUserProvidersDto, IdentityUserChangePasswordDto,
            IdentityRoleClaimsDto, IdentityUserClaimDto, IdentityRoleClaimDto>(ConfigureUIOptions);

        // Monitor changes in Admin UI views
        services.AddAdminUIRazorRuntimeCompilation(HostingEnvironment);

        // Add email senders which is currently setup for SendGrid and SMTP
        services.AddEmailSenders(Configuration);
        services.AddInfrastructure(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseRouting();

        app.UseIdentityServerAdminUI();

        app.UseEndpoints(endpoint =>
        {
            endpoint.MapIdentityServerAdminUI();
            endpoint.MapIdentityServerAdminUIHealthChecks();
        });
    }

    public virtual void ConfigureUIOptions(IdentityServerAdminUIOptions options)
    {
        // Applies configuration from appsettings.
        options.BindConfiguration(Configuration);
        if (HostingEnvironment.IsDevelopment())
        {
            options.Security.UseDeveloperExceptionPage = true;
        }
        else
        {
            options.Security.UseHsts = true;
        }

        // Set migration assembly for application of db migrations
        var migrationsAssembly = typeof(Infrastructure.Marker).GetTypeInfo().Assembly.GetName().Name;
        options.DatabaseMigrations.SetMigrationsAssemblies(migrationsAssembly);

        // Use production DbContexts and auth services.
        options.Testing.IsStaging = false;
    }
}