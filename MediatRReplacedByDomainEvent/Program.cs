using MediatRReplacedByDomainEvent;
using MediatRReplacedByDomainEvent.Command;
using MediatRReplacedByDomainEvent.Data;
using MediatRReplacedByDomainEvent.Dtos;
using MediatRReplacedByDomainEvent.Models;
using MediatRReplacedByDomainEvent.Query;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(loggingBuilder => {
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

builder.Services.AddOpenApi();

// 配置内存数据库
//var connection = new SqliteConnection("Data Source=:memory:");
//connection.Open();

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite(connection)
//           .LogTo(Console.WriteLine, LogLevel.Information));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ECommerceDB"));

// 配置 JSON 序列化选项 - 修复 GUID 转换问题
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringGuidConverter());
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddCommandEventSystem();

var app = builder.Build();

// 初始化测试数据
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = services.GetRequiredService<AppDbContext>();

        if (!db.Products.Any())
        {
            // 使用四个参数的构造函数
            var laptop = new Product(
                new Guid("37386ba8-b8c7-4c10-92f7-bf3bc1b84138"),
                "Laptop",
                1200.00m,
                10);

            var smartphone = new Product(
                new Guid("39533c72-51ed-4b11-a9fb-682cdf3cc64e"),
                "Smartphone",
                800.00m,
                20);

            var headphones = new Product(
                new Guid("c20d51ab-527d-4a8f-8920-9fd46c273880"),
                "Headphones",
                150.00m,
                50);

            db.Products.AddRange(laptop, smartphone, headphones);
            await db.SaveChangesAsync();

            logger.LogInformation("Added sample products with IDs: \n" +
                                  $"- Laptop: {laptop.Id}\n" +
                                  $"- Smartphone: {smartphone.Id}\n" +
                                  $"- Headphones: {headphones.Id}");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Database.EnsureCreated();
//}

app.UseHttpsRedirection();

// 命令端点组
var commandGroup = app.MapGroup("/commands");
commandGroup.MapPost("/orders", async (CreateOrderCommand command, ICommandDispatcher dispatcher) =>
{
    await dispatcher.SendAysnc(command);
    return Results.Created($"/orders/{command}", null);
});

commandGroup.MapPost("/orders/{id}/pay", async (Guid id, ICommandDispatcher dispatcher) =>
{
    await dispatcher.SendAysnc(new PayOrderCommand(id));
    return Results.Ok();
});

commandGroup.MapPost("/orders/{id}/cancel", async (Guid id, ICommandDispatcher dispatcher) =>
{
    await dispatcher.SendAysnc(new CancelOrderCommand(id));
    return Results.Ok();
});

commandGroup.MapPost("/products/{id}/restock", async (Guid id, int quantity, ICommandDispatcher dispatcher) =>
{
    await dispatcher.SendAysnc(new RestockProductCommand(id, quantity));
    return Results.Ok();
});

var queryGroup = app.MapGroup("/queries");
queryGroup.MapGet("/orders/{id}", async (Guid id, IQueryDispatcher dispatcher) =>
{
    var result = await dispatcher.QueryAsync<GetOrderByIdQuery, OrderDto>(new GetOrderByIdQuery(id));
    return result != null ? Results.Ok(result) : Results.NotFound();
});

queryGroup.MapGet("/orders", async (string email, IQueryDispatcher dispatcher) =>
{
    var result = await dispatcher.QueryAsync<GetUserOrdersQuery, IEnumerable<OrderDto>>(
        new GetUserOrdersQuery(email));
    return Results.Ok(result);
});

queryGroup.MapGet("/products", async (IQueryDispatcher dispatcher) =>
{
    var result = await dispatcher.QueryAsync<GetAllProductsQuery, IEnumerable<ProductDto>>(
        new GetAllProductsQuery());
    return Results.Ok(result);
});


app.Run();


