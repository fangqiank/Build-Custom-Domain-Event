using MediatRReplacedByDomainEvent;
using MediatRReplacedByDomainEvent.Data;
using MediatRReplacedByDomainEvent.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 配置内存数据库
//builder.Services.AddDbContext<OrderDbContext>(options =>
//    options.UseInMemoryDatabase("OrdersDB")
//           .ConfigureWarnings(warnings =>
//               warnings.Ignore(CoreEventId.TransactionIgnoredWarning)));

var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite(connection)
           .LogTo(Console.WriteLine, LogLevel.Information));

// 注册自定义命令/事件系统
builder.Services.AddCommandEventSystem();

// 注册应用服务
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async (
    [FromBody] CreateOrderRequest request,
    [FromServices] OrderService service
) => {
    var result = await service.CreateOrder(request);
    return Results.Created($"/orders/{result.OrderId}", result);
});

app.Run();


