using DomainDrivenDesignShop.Domain.Entities;
using DomainDrivenDesignShop.Domain.Repositories;

namespace DomainDrivenDesignShop.Application;

public sealed record CreateOrderCommand(string Currency);
public sealed record CreateOrderResult(Guid OrderId);


public sealed class CreateOrderHandler(IOrderRepository orders, IUnitOfWork uow)
{
    private readonly IOrderRepository _orders = orders;
    private readonly IUnitOfWork _uow = uow;

    public async Task<CreateOrderResult> HandleAsync(CreateOrderCommand cmd, CancellationToken ct = default)
    {
        var order = Order.Create(cmd.Currency);
        await _orders.AddAsync(order, ct);
        await _uow.SaveChangesAsync(ct);
        return new CreateOrderResult(order.Id);
    }
}
