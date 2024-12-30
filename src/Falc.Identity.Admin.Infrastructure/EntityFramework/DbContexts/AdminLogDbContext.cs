// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.EntityFrameworkCore;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Constants;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Interfaces;

namespace Falc.Identity.Admin.Infrastructure.EntityFramework.DbContexts;

public class AdminLogDbContext(DbContextOptions<AdminLogDbContext> options) : DbContext(options), IAdminLogDbContext
{
    public DbSet<Log> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureLogContext(builder);
    }

    private void ConfigureLogContext(ModelBuilder builder)
    {
        builder.Entity<Log>(log =>
        {
            log.ToTable(TableConsts.Logging);
            log.HasKey(x => x.Id);
            log.Property(x => x.Level).HasMaxLength(128);
        });
    }
}