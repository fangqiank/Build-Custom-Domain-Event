using FluentValidation;
using MediatRReplacedByDomainEvent.Command;
using MediatRReplacedByDomainEvent.Dtos;
using MediatRReplacedByDomainEvent.Query;
using System.Reflection;

namespace MediatRReplacedByDomainEvent
{

    public static class ServiceCollectionExtensions //Scrutor package
    {
        /// <summary>
        /// 注册自定义命令/事件系统
        /// </summary>
        public static IServiceCollection AddCommandEventSystem(this IServiceCollection services)
        {
            //// 注册调度器
            services.AddScoped<Dispatcher>();
            services.AddScoped<ICommandDispatcher>(sp => sp.GetRequiredService<Dispatcher>());
            services.AddScoped<IQueryDispatcher>(sp => sp.GetRequiredService<Dispatcher>());
            services.AddScoped<IEventDispatcher>(sp => sp.GetRequiredService<Dispatcher>());

            // 注册命令处理器 - 使用更健壮的注册方式
            services.AddScoped<ICommandHandler<CreateOrderCommand>, CreateOrderHandler>();
            services.AddScoped<ICommandHandler<PayOrderCommand>, PayOrderHandler>();
            services.AddScoped<ICommandHandler<CancelOrderCommand>, CancelOrderHandler>();
            services.AddScoped<ICommandHandler<RestockProductCommand>, RestockProductHandler>();

            // 注册查询处理器
            services.AddScoped<IQueryHandler<GetOrderByIdQuery, OrderDto>, GetOrderByIdHandler>();
            services.AddScoped<IQueryHandler<GetUserOrdersQuery, IEnumerable<OrderDto>>, GetUserOrdersHandler>();
            services.AddScoped<IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>, GetAllProductsHandler>();

            // 注册事件处理器 - 确保所有依赖都能解析
            services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedHandler>();
            services.AddScoped<IEventHandler<OrderPaidEvent>, OrderPaidHandler>();
            services.AddScoped<IEventHandler<OrderCancelledEvent>, OrderCancelledHandler>();
            services.AddScoped<IEventHandler<StockReducedEvent>, StockReducedHandler>();

            // 注册验证器
            services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
            services.AddScoped<IValidator<GetUserOrdersQuery>, GetUserOrdersQueryValidator>();

            // 注册管道行为 - 全局行为
            // 日志管道行为
            services.AddTransient(typeof(IPipelineBehavior<>), typeof(LoggingPipelineBehavior<>));
            services.AddTransient(typeof(IQueryPipelineBehavior<,>), typeof(LoggingQueryPipelineBehavior<,>));

            // 验证管道行为
            services.AddTransient(typeof(IPipelineBehavior<>), typeof(ValidationPipelineBehavior<>));
            services.AddTransient(typeof(IQueryPipelineBehavior<,>), typeof(ValidationQueryPipelineBehavior<,>));

            // 事务管道行为（仅命令）
            services.AddTransient(typeof(IPipelineBehavior<>), typeof(TransactionPipelineBehavior<>));

            // 性能监控管道行为
            services.AddTransient(typeof(IPipelineBehavior<>), typeof(PerformancePipelineBehavior<>));
            services.AddTransient(typeof(IQueryPipelineBehavior<,>), typeof(PerformanceQueryPipelineBehavior<,>));

            // 注册模拟服务
            services.AddSingleton<IEmailService, ConsoleEmailService>();
            services.AddSingleton<IShippingService, MockShippingService>();
            services.AddSingleton<IInventoryService, MockInventoryService>();

            return services;
        }
    }
}
    

    /*
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandEventSystem(this IServiceCollection services)
        {
            // 注册核心分发器
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();

            // 手动注册开放泛型管道行为
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionPipelineBehavior<,>));

            // 获取当前程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 手动注册命令处理器
            RegisterImplementations(services, assembly, typeof(ICommandHandler<,>));

            // 手动注册事件处理器
            RegisterImplementations(services, assembly, typeof(IEventHandler<>));

            // 手动注册管道行为
            RegisterImplementations(services, assembly, typeof(IPipelineBehavior<,>));

            // 注册验证器
            RegisterValidators(services, assembly);

            return services;
        }

        private static void RegisterImplementations(
            IServiceCollection services,
            Assembly assembly,
            Type serviceType)
        {
            // 获取所有实现指定接口的具体类
            var implementations = assembly.GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == serviceType))
                .ToList();

            foreach (var impl in implementations)
            {
                // 获取实现的接口
                var interfaces = impl.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == serviceType);

                foreach (var interfaceType in interfaces)
                {
                    services.AddScoped(interfaceType, impl);
                }
            }
        }

        private static void RegisterValidators(IServiceCollection services, Assembly assembly)
        {
            // 注册 FluentValidation 验证器
            var validatorTypes = assembly.GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.BaseType != null &&
                    t.BaseType.IsGenericType &&
                    t.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>));

            foreach (var validatorType in validatorTypes)
            {
                var validatedType = validatorType.BaseType!.GetGenericArguments()[0];
                var validatorInterface = typeof(IValidator<>).MakeGenericType(validatedType);

                services.AddScoped(validatorInterface, validatorType);
            }
        }
    }
}
    */
