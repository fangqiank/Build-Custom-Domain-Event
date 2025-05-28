using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sample2;
using Sample2.Data;
using Sample2.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 添加数据库上下文（使用内存数据库）
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseInMemoryDatabase("OrdersDB")
           // 添加以下配置忽略事务警告
           .ConfigureWarnings(w =>
               w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

// 注册领域事件系统
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.Scan(scan => scan
    .FromAssemblyOf<OrderCreatedEvent>()
    .AddClasses(c => c.AssignableToAny(typeof(IEventHandler<>), typeof(IAsyncEventHandler<>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// 注册事务装饰器
builder.Services.Decorate<IEventDispatcher, TransactionalEventDispatcher>();

builder.Services.AddScoped<OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async (
    [FromBody] CreateOrderRequest request,
    [FromServices] OrderService service) =>
{
    try
    {
        var order = await service.CreateOrder(request);
        return Results.Created($"/orders/{order.Id}", order);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();

