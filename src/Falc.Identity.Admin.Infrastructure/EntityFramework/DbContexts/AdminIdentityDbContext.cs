// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Falc.Identity.Admin.Infrastructure.EntityFramework.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Falc.Identity.Admin.Infrastructure.EntityFramework.DbContexts;

public class AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext> options)
    : IdentityDbContext<UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
        UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>(options)
{
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