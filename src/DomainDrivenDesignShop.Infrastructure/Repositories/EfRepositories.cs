using DomainDrivenDesignShop.Domain.Entities;
using DomainDrivenDesignShop.Domain.Repositories;
using DomainDrivenDesignShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenDesignShop.Infrastructure.Repositories;

public sealed class EfProductRepository : IProductRepository
{
    private readonly AppDbContext _db;
    public EfProductRepository(AppDbContext db) => _db = db;


    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);


    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
        => await _db.Products.AsNoTracking().ToListAsync(ct);


    public Task AddAsync(Product product, CancellationToken ct = default)
        => _db.Products.AddAsync(product, ct).AsTask();


    public void Update(Product product)
        => _db.Products.Update(product);


    public void Remove(Product product)
        => _db.Products.Remove(product);
}


public sealed class EfOrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    public EfOrderRepository(AppDbContext db) => _db = db;


    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct);


    public async Task AddAsync(Order order, CancellationToken ct = default)
        => await _db.Orders.AddAsync(order, ct);


    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
        => await _db.Orders.Include(o => o.Lines)
                           .AsNoTracking()
                           .ToListAsync(ct);


    public void Update(Order order)
        => _db.Orders.Update(order);


    public void Remove(Order order)
        => _db.Orders.Remove(order);
}


public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    public EfUnitOfWork(AppDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}