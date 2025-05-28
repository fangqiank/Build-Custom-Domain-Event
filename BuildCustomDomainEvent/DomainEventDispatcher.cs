using BuildCustomDomainEvent.Models;

namespace BuildCustomDomainEvent
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch(object source, object @event);
    }
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Dispatch(object source, object @event)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());

            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                await (Task)handlerType.GetMethod("Handle")!
                    .Invoke(handler, new object[] { @event })!;
            }
        }
    }
}
