using ITC.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Internal;

namespace ITC.ReportService.Database
{
    public class EntityBaseInterceptor : SaveChangesInterceptor
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
            if (eventData.Context is not null)
            {
                var newEntities = eventData.Context.ChangeTracker.Entries()
                    .Where(
                        x => x.State == EntityState.Added &&
                             x.Entity != null &&
                             x.Entity is EntityBase
                    )
                    .Select(x => x.Entity as EntityBase);

                var modifiedEntries = eventData.Context.ChangeTracker.Entries()
                    .Where(
                        x => x.State == EntityState.Modified &&
                             x.Entity != null &&
                             x.Entity is EntityBase
                    );

                var deletedEntities = eventData.Context.ChangeTracker.Entries()
                    .Where(
                        x => x.State == EntityState.Deleted &&
                             x.Entity != null &&
                             x.Entity is EntityBase
                    );

                foreach (var newEntity in newEntities)
                {
                    if (newEntity != null)
                    {
                        if (newEntity.CreatedAt == default)
                            newEntity.CreatedAt = _systemClock.UtcNow;
                    }
                }

                foreach (var entry in modifiedEntries)
                {
                    if (entry is { State: EntityState.Modified, Entity: EntityBase modify })
                    {
                        eventData.Context.Entry(modify).Property(p => p.CreatedAt).IsModified = false;

                        eventData.Context.Entry(modify).Property(p => p.DeletedAt).IsModified = false;

                        modify.UpdatedAt = _systemClock.UtcNow;
                    }
                }

                foreach (var entry in deletedEntities)
                {
                    if (entry is { State: EntityState.Deleted, Entity: EntityBase delete })
                    {
                        entry.State = EntityState.Modified;
                        delete.DeletedAt = _systemClock.UtcNow;
                    }
                }
            }

            return result;
        }
    }
}
