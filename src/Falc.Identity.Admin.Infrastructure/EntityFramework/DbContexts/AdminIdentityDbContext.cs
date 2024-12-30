// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Falc.Identity.Admin.Infrastructure.EntityFramework.Entities.Identity;
using Falc.Identity.Admin.Infrastructure.EntityFramework.Interceptors;
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Falc.Identity.Admin.Infrastructure.EntityFramework.DbContexts;

public class AdminIdentityDbContext
    : IdentityDbContext<UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
        UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>
{
    private readonly IPublishEndpoint _publishEndpoint;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(new PublishUserCreatedEventInterceptor(_publishEndpoint));
    }

#pragma warning disable CS8618, CS9264
    public AdminIdentityDbContext() { }
    public AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext> options) : base(options) { }
#pragma warning restore CS8618, CS9264
    
    public AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext> options, IPublishEndpoint publishEndpoint) : base(options)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentityContext(builder);
    }

    private void ConfigureIdentityContext(ModelBuilder builder)
    {
        builder.Entity<UserIdentityRole>().ToTable("Roles");
        builder.Entity<UserIdentityRoleClaim>().ToTable("RoleClaims");
        builder.Entity<UserIdentityUserRole>().ToTable("UserRoles");
        builder.Entity<UserIdentity>().ToTable("Users");
        builder.Entity<UserIdentityUserLogin>().ToTable("UserLogins");
        builder.Entity<UserIdentityUserClaim>().ToTable("UserClaims");
        builder.Entity<UserIdentityUserToken>().ToTable("UserTokens");
    }
}