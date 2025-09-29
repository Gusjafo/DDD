using DomainDrivenDesignShop.Domain.Abstractions;
using DomainDrivenDesignShop.Domain.Repositories;

namespace DomainDrivenDesignShop.Application;


public sealed record AddProductToOrderCommand(Guid OrderId, Guid ProductId, int Quantity);


public sealed class AddProductToOrderHandler(IOrderRepository orders, IProductRepository products, IUnitOfWork uow)
{
    private readonly IOrderRepository _orders = orders;
    private readonly IProductRepository _products = products;
    private readonly IUnitOfWork _uow = uow;

    public async Task HandleAsync(AddProductToOrderCommand cmd, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
        ?? throw DomainErrors.NotFound($"Pedido {cmd.OrderId}");


        var product = await _products.GetByIdAsync(cmd.ProductId, ct)
        ?? throw DomainErrors.NotFound($"Producto {cmd.ProductId}");


        order.AddProduct(product, cmd.Quantity);
        await _uow.SaveChangesAsync(ct);
    }
}