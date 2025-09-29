using DomainDrivenDesignShop.Application;
using DomainDrivenDesignShop.Domain.Repositories;
using DomainDrivenDesignShop.Infrastructure.Persistence;
using DomainDrivenDesignShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// EF Core — SQLite para persistencia local
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=dddshop.db";
    opt.UseSqlite(connectionString);
});

// Repositorios (puertos)
builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// Casos de uso
builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddScoped<AddProductToOrderHandler>();
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<UpdateProductHandler>();
builder.Services.AddScoped<DeleteProductHandler>();
builder.Services.AddScoped<GetProductHandler>();
builder.Services.AddScoped<ListProductsHandler>();
builder.Services.AddScoped<GetOrderHandler>();
builder.Services.AddScoped<ListOrdersHandler>();
builder.Services.AddScoped<UpdateOrderHandler>();
builder.Services.AddScoped<DeleteOrderHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
