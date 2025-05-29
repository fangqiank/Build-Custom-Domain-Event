using FluentValidation;
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
            // 注册核心分发器
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();

            // 自动注册命令处理器
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // 自动注册事件处理器
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // 自动注册管道行为
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.AssignableTo(typeof(IPipelineBehavior<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // 注册验证器（使用 FluentValidation）
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);

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
