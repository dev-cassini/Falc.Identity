using Falc.Identity.Admin.Infrastructure.EntityFramework.Entities.Identity;
using Falc.Identity.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Falc.Identity.Admin.Infrastructure.EntityFramework.Interceptors;

public class PublishUserCreatedEventInterceptor(IPublishEndpoint publishEndpoint) : ISaveChangesInterceptor
{
    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return await ValueTask.FromResult(result);
        
        var addedUserEntries = eventData.Context.ChangeTracker
            .Entries()
            .Where(x => x.State is EntityState.Added)
            .Where(x => x.Entity is UserIdentity);
        
        foreach (var addedUserEntry in addedUserEntries)
        {
            if (addedUserEntry.Entity is not UserIdentity userIdentity) continue;
            if (userIdentity.Email is null or "") continue;
            
            var userCreatedEvent = new UserCreated(userIdentity.Id, userIdentity.Email);
            await publishEndpoint.Publish(userCreatedEvent, cancellationToken);
        }

        return await ValueTask.FromResult(result);
    }
}