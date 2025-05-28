using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sample2.Data;
using Sample2.Models;

namespace Sample2
{
    public class EventPublishingInterceptor : SaveChangesInterceptor
    {
        private readonly IEventDispatcher _eventDispatcher;

        public EventPublishingInterceptor(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is DbContext context)
            {
                var entries = context.ChangeTracker
                    .Entries<EntityBase>()
                    .Where(e => e.Entity.DomainEvents.Any());

                var events = entries
                    .SelectMany(e => e.Entity.DomainEvents)
                    .ToList();

                foreach (var domainEvent in events)
                {
                    await _eventDispatcher.DispatchAsync(domainEvent);
                }

                foreach (var entry in entries)
                {
                    entry.Entity.ClearDomainEvents();
                }
            }

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }
    }

}
