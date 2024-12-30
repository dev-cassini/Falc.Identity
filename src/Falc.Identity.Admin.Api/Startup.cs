// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.IdentityModel.Tokens.Jwt;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag.AspNetCore;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Falc.Identity.Admin.Api.Configuration;
using Falc.Identity.Admin.EntityFramework.Shared.DbContexts;
using Falc.Identity.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.Duende.IdentityServer.Admin.UI.Api.Configuration;
using Skoruba.Duende.IdentityServer.Admin.UI.Api.Helpers;
using Skoruba.Duende.IdentityServer.Shared.Configuration.Helpers;
using Falc.Identity.Shared.Dtos;
using Falc.Identity.Shared.Dtos.Identity;
using IdentityModel;

namespace Falc.Identity.Admin.Api;

public class Startup
{
    public Startup(IWebHostEnvironment env, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        HostingEnvironment = env;
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public IWebHostEnvironment HostingEnvironment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var adminApiConfiguration = Configuration.GetSection(nameof(AdminApiConfiguration)).Get<AdminApiConfiguration>();
        services.AddSingleton(adminApiConfiguration);
            
        // Add DbContexts
        RegisterDbContexts(services);
            
        // Add email senders which is currently setup for SendGrid and SMTP
        services.AddEmailSenders(Configuration);
   
        // Add authentication services
        RegisterAuthentication(services);

        // Add authorization services
        RegisterAuthorization(services);
            
        services.AddIdentityServerAdminApi<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, IdentityServerDataProtectionDbContext,AdminLogDbContext, AdminAuditLogDbContext, AuditLog,
            IdentityUserDto, IdentityRoleDto, UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
            UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
            IdentityUsersDto, IdentityRolesDto, IdentityUserRolesDto,
            IdentityUserClaimsDto, IdentityUserProviderDto, IdentityUserProvidersDto, IdentityUserChangePasswordDto,
            IdentityRoleClaimsDto, IdentityUserClaimDto, IdentityRoleClaimDto>(Configuration, adminApiConfiguration);

        services.AddSwaggerServices(adminApiConfiguration);
            
        services.AddIdSHealthChecks<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminIdentityDbContext, AdminLogDbContext, AdminAuditLogDbContext, IdentityServerDataProtectionDbContext>(Configuration, adminApiConfiguration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AdminApiConfiguration adminApiConfiguration)
    {
        app.AddForwardHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseOpenApi();
        app.UseSwaggerUi(settings =>
        {
            settings.OAuth2Client = new OAuth2ClientSettings
            {
                ClientId = adminApiConfiguration.OidcSwaggerUIClientId,
                AppName = adminApiConfiguration.ApiName,
                UsePkceWithAuthorizationCodeGrant = true,
                ClientSecret = null
            };
        });

        app.UseRouting();
        UseAuthentication(app);
        app.UseCors();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });
    }

    public virtual void RegisterDbContexts(IServiceCollection services)
    {
        services.AddDbContexts<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext, IdentityServerDataProtectionDbContext, AuditLog>(Configuration);
    }

    public virtual void RegisterAuthentication(IServiceCollection services)
    {
        services.AddApiAuthentication<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(Configuration);
    }

    public virtual void RegisterAuthorization(IServiceCollection services)
    {
        var adminApiConfiguration = services.BuildServiceProvider().GetService<AdminApiConfiguration>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "RequireAdministratorRole",
                policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                {
                    if (c.Type == "role" && c.Value == adminApiConfiguration.AdministrationRole)
                        return true;
                    return c.Type == "client_role" && c.Value == adminApiConfiguration.AdministrationRole;
                }) && context.User.HasClaim(c =>
                    c.Type == "scope" && c.Value == adminApiConfiguration.OidcApiName)));
                
            options.AddPolicy("Me", policy =>
            {
                policy.RequireClaim(JwtClaimTypes.Scope, "falc-identity:me");
            });
        });
    }

    public virtual void UseAuthentication(IApplicationBuilder app)
    {
        app.UseAuthentication();
    }
}