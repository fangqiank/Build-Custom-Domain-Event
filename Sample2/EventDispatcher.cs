using System;

namespace Sample2
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent;
    }

    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventDispatcher> _logger;

        public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
        {
            try
            {
                _logger.LogInformation("开始处理 {EventType} 事件", typeof(TEvent).Name);

                // 处理同步处理程序
                var syncHandlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();
                foreach (var handler in syncHandlers)
                {
                    _logger.LogDebug("执行同步处理程序：{HandlerType}", handler.GetType().Name);
                    await handler.Handle(@event);
                }

                // 处理异步处理程序
                var asyncHandlers = _serviceProvider.GetServices<IAsyncEventHandler<TEvent>>();
                var tasks = asyncHandlers.Select(async handler =>
                {
                    _logger.LogDebug("执行异步处理程序：{HandlerType}", handler.GetType().Name);
                    await handler.HandleAsync(@event);
                });

                await Task.WhenAll(tasks);

                _logger.LogInformation("事件 {EventType} 处理完成", typeof(TEvent).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理事件 {EventType} 时发生错误", typeof(TEvent).Name);
                throw;
            }
        }
    }
}
