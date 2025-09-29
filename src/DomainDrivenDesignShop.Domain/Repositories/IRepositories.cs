using DomainDrivenDesignShop.Domain.Entities;

namespace DomainDrivenDesignShop.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
}


public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
}


public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}