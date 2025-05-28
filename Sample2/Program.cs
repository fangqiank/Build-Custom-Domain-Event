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

// ������ݿ������ģ�ʹ���ڴ����ݿ⣩
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseInMemoryDatabase("OrdersDB")
           // ����������ú������񾯸�
           .ConfigureWarnings(w =>
               w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

// ע�������¼�ϵͳ
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.Scan(scan => scan
    .FromAssemblyOf<OrderCreatedEvent>()
    .AddClasses(c => c.AssignableToAny(typeof(IEventHandler<>), typeof(IAsyncEventHandler<>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// ע������װ����
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

