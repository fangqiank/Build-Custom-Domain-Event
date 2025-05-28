using BuildCustomDomainEvent;
using BuildCustomDomainEvent.Data;
using BuildCustomDomainEvent.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("OrderDb"));

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

builder.Services.AddTransient<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEmailHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async (string customerName, decimal totalAmount, OrderService orderService) =>
{
    var orderId = await orderService.CreateOrder(customerName, totalAmount);
    return Results.Created($"/orders/{orderId}", new { Id = orderId });
})
.WithName("CreateOrder")
.WithOpenApi();

app.Run();


