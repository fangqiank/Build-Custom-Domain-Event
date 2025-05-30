namespace MediatRReplacedByDomainEvent
{
    public interface IEventDispatcher
    {
        Task PublishAsync(IDomainEvent domainEvent, CancellationToken ct = default);
    }

    //public class EventDispatcher(
    //    IServiceProvider serviceProvider,
    //    ILogger<EventDispatcher> logger
    //    ) : IEventDispatcher
    //{
    //    public async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken ct = default)
    //        where TEvent : IDomainEvent
    //    {
    //        logger.LogInformation("Dispatching event: {EventType}", typeof(TEvent).Name);
    //        try
    //        {
    //            var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>().ToList();

    //            var tasks = handlers.Select(handler =>
    //            {
    //                logger.LogInformation("Handling event {EventType} with handler {HandlerType}", 
    //                    typeof(TEvent).Name, handler.GetType().Name);
    //                return handler.Handle(@event, ct);
    //            });

    //            await Task.WhenAll(tasks);
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogError(ex, "Error dispatching event: {EventType}", typeof(TEvent).Name);
    //            throw;
    //        }
    //    }
    //}
}
            
