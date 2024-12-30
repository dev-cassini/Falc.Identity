// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Interfaces;

namespace Falc.Identity.Admin.Infrastructure.EntityFramework.DbContexts;

public class IdentityServerPersistedGrantDbContext(DbContextOptions<IdentityServerPersistedGrantDbContext> options)
    : PersistedGrantDbContext<IdentityServerPersistedGrantDbContext>(options), IAdminPersistedGrantDbContext;