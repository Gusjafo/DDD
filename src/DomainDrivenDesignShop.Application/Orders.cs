using System.Collections.Generic;
using System.Linq;
using DomainDrivenDesignShop.Domain.Abstractions;
using DomainDrivenDesignShop.Domain.Entities;
using DomainDrivenDesignShop.Domain.Repositories;

namespace DomainDrivenDesignShop.Application;

public sealed record OrderLineDto(Guid Id, Guid ProductId, int Quantity, decimal UnitPrice, string Currency, decimal Subtotal);
public sealed record OrderDto(Guid Id, DateTime CreatedAtUtc, string Currency, IReadOnlyList<OrderLineDto> Lines, decimal TotalAmount);

public sealed record GetOrderQuery(Guid OrderId);

public sealed class GetOrderHandler(IOrderRepository orders)
{
    private readonly IOrderRepository _orders = orders;

    public async Task<OrderDto> HandleAsync(GetOrderQuery query, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(query.OrderId, ct)
            ?? throw DomainErrors.NotFound($"Pedido {query.OrderId}");

        return Map(order);
    }

    private static OrderDto Map(Order order)
    {
        var lines = order.Lines
            .Select(l => new OrderLineDto(l.Id, l.ProductId, l.Quantity, l.UnitPrice.Amount, l.UnitPrice.Currency, l.Subtotal().Amount))
            .ToList();

        return new OrderDto(order.Id, order.CreatedAtUtc, order.Currency, lines, order.Total().Amount);
    }
}


public sealed record ListOrdersQuery;

public sealed class ListOrdersHandler(IOrderRepository orders)
{
    private readonly IOrderRepository _orders = orders;

    public async Task<IReadOnlyList<OrderDto>> HandleAsync(ListOrdersQuery query, CancellationToken ct = default)
    {
        var all = await _orders.GetAllAsync(ct);
        return all.Select(Map).ToList();
    }

    private static OrderDto Map(Order order)
    {
        var lines = order.Lines
            .Select(l => new OrderLineDto(l.Id, l.ProductId, l.Quantity, l.UnitPrice.Amount, l.UnitPrice.Currency, l.Subtotal().Amount))
            .ToList();

        return new OrderDto(order.Id, order.CreatedAtUtc, order.Currency, lines, order.Total().Amount);
    }
}


public sealed record UpdateOrderCommand(Guid OrderId, string Currency);

public sealed class UpdateOrderHandler(IOrderRepository orders, IUnitOfWork uow)
{
    private readonly IOrderRepository _orders = orders;
    private readonly IUnitOfWork _uow = uow;

    public async Task HandleAsync(UpdateOrderCommand cmd, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
            ?? throw DomainErrors.NotFound($"Pedido {cmd.OrderId}");

        order.UpdateCurrency(cmd.Currency);
        _orders.Update(order);
        await _uow.SaveChangesAsync(ct);
    }
}


public sealed record DeleteOrderCommand(Guid OrderId);

public sealed class DeleteOrderHandler(IOrderRepository orders, IUnitOfWork uow)
{
    private readonly IOrderRepository _orders = orders;
    private readonly IUnitOfWork _uow = uow;

    public async Task HandleAsync(DeleteOrderCommand cmd, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
            ?? throw DomainErrors.NotFound($"Pedido {cmd.OrderId}");

        _orders.Remove(order);
        await _uow.SaveChangesAsync(ct);
    }
}
