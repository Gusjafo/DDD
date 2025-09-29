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
    => _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
}


public sealed class EfOrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    public EfOrderRepository(AppDbContext db) => _db = db;


    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    => _db.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, ct);


    public async Task AddAsync(Order order, CancellationToken ct = default)
    => await _db.Orders.AddAsync(order, ct);
}


public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    public EfUnitOfWork(AppDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}