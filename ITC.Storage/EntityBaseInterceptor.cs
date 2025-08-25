using ITC.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Internal;

namespace ITC.Storage;

internal class EntityBaseInterceptor : SaveChangesInterceptor
{
    private readonly ISystemClock _systemClock;

    public EntityBaseInterceptor(ISystemClock systemClock)
    {
        _systemClock = systemClock;
    }
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        result = SetDates(eventData, result);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        result = SetDates(eventData, result);
            
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private InterceptionResult<int> SetDates(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is null) return result;

        var newEntities = eventData.Context.ChangeTracker.Entries()
            .Where(
                x => x.State == EntityState.Added &&
                     x.Entity is EntityBase
            )
            .Select(x => x.Entity as EntityBase);

        var modifiedEntries = eventData.Context.ChangeTracker.Entries()
            .Where(
                x => x.State == EntityState.Modified &&
                     x.Entity is EntityBase
            );

        var deletedEntities = eventData.Context.ChangeTracker.Entries()
            .Where(
                x => x.State == EntityState.Deleted &&
                     x.Entity is EntityBase
            );

        foreach (var newEntity in newEntities)
        {
            if (newEntity == null) continue;
            if (newEntity.CreatedAt == default)
                newEntity.CreatedAt = _systemClock.UtcNow;
            newEntity.Timestamp = newEntity.CreatedAt.Ticks;
        }

        foreach (var entry in modifiedEntries)
        {
            if (entry is not { State: EntityState.Modified, Entity: EntityBase modify }) continue;
            eventData.Context.Entry(modify).Property(p => p.CreatedAt).IsModified = false;

            eventData.Context.Entry(modify).Property(p => p.DeletedAt).IsModified = false;

            modify.UpdatedAt = _systemClock.UtcNow;
            modify.Timestamp = modify.UpdatedAt.Value.Ticks;
        }

        foreach (var entry in deletedEntities)
        {
            if (entry is not { State: EntityState.Deleted, Entity: EntityBase delete }) continue;
            entry.State = EntityState.Modified;
            delete.DeletedAt = _systemClock.UtcNow;
        }

        return result;
    }
}