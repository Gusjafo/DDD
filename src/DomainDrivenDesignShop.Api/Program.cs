using DomainDrivenDesignShop.Application;
using DomainDrivenDesignShop.Domain.Entities;
using DomainDrivenDesignShop.Domain.Repositories;
using DomainDrivenDesignShop.Domain.ValueObjects;
using DomainDrivenDesignShop.Infrastructure.Persistence;
using DomainDrivenDesignShop.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// EF Core — InMemory para demo (puedes cambiar a SQL Server, PostgreSQL, etc.)
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("dddshop"));

// Repositorios (puertos)
builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// Casos de uso
builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddScoped<AddProductToOrderHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async ([FromBody] CreateOrderRequest req, CreateOrderHandler handler, CancellationToken ct) =>
{
    var result = await handler.HandleAsync(new CreateOrderCommand(req.Currency), ct);
    return Results.Created($"/orders/{result.OrderId}", result);
});


app.MapPost("/orders/{orderId:guid}/lines", async (Guid orderId, [FromBody] AddLineRequest req, AddProductToOrderHandler handler, CancellationToken ct) =>
{
    await handler.HandleAsync(new AddProductToOrderCommand(orderId, req.ProductId, req.Quantity), ct);
    return Results.NoContent();
});


// Semilla opcional para probar rápido
app.MapPost("/seed", async (AppDbContext db) =>
{
    var p1 = Product.Create("Café 250g", Money.From(5.90m, "EUR"));
    var p2 = Product.Create("Té Verde 100g", Money.From(3.50m, "EUR"));
    db.Products.AddRange(p1, p2);
    await db.SaveChangesAsync();
    return Results.Ok(new { Product1Id = p1.Id, Product2Id = p2.Id });
});


app.Run();


// DTOs API
public sealed record CreateOrderRequest(string Currency);
public sealed record AddLineRequest(Guid ProductId, int Quantity);