// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.EntityFrameworkCore;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;

namespace Falc.Identity.Admin.Infrastructure.EntityFramework.DbContexts;

public class AdminAuditLogDbContext(DbContextOptions<AdminAuditLogDbContext> dbContextOptions)
    : DbContext(dbContextOptions), IAuditLoggingDbContext<AuditLog>
{
    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    public DbSet<AuditLog> AuditLog { get; set; }
}