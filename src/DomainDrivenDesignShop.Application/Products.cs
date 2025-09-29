using System.Collections.Generic;
using System.Linq;
using DomainDrivenDesignShop.Domain.Abstractions;
using DomainDrivenDesignShop.Domain.Entities;
using DomainDrivenDesignShop.Domain.Repositories;
using DomainDrivenDesignShop.Domain.ValueObjects;

namespace DomainDrivenDesignShop.Application;

public sealed record ProductDto(Guid Id, string Name, decimal Amount, string Currency);

public sealed record CreateProductCommand(string Name, decimal Amount, string Currency);
public sealed record CreateProductResult(Guid ProductId);

public sealed class CreateProductHandler(IProductRepository products, IUnitOfWork uow)
{
    private readonly IProductRepository _products = products;
    private readonly IUnitOfWork _uow = uow;

    public async Task<CreateProductResult> HandleAsync(CreateProductCommand cmd, CancellationToken ct = default)
    {
        var money = Money.From(cmd.Amount, cmd.Currency);
        var product = Product.Create(cmd.Name, money);
        await _products.AddAsync(product, ct);
        await _uow.SaveChangesAsync(ct);
        return new CreateProductResult(product.Id);
    }
}


public sealed record UpdateProductCommand(Guid ProductId, string Name, decimal Amount, string Currency);

public sealed class UpdateProductHandler(IProductRepository products, IUnitOfWork uow)
{
    private readonly IProductRepository _products = products;
    private readonly IUnitOfWork _uow = uow;

    public async Task HandleAsync(UpdateProductCommand cmd, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(cmd.ProductId, ct)
            ?? throw DomainErrors.NotFound($"Producto {cmd.ProductId}");

        product.Update(cmd.Name, Money.From(cmd.Amount, cmd.Currency));
        _products.Update(product);
        await _uow.SaveChangesAsync(ct);
    }
}


public sealed record DeleteProductCommand(Guid ProductId);

public sealed class DeleteProductHandler(IProductRepository products, IUnitOfWork uow)
{
    private readonly IProductRepository _products = products;
    private readonly IUnitOfWork _uow = uow;

    public async Task HandleAsync(DeleteProductCommand cmd, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(cmd.ProductId, ct)
            ?? throw DomainErrors.NotFound($"Producto {cmd.ProductId}");

        _products.Remove(product);
        await _uow.SaveChangesAsync(ct);
    }
}


public sealed record GetProductQuery(Guid ProductId);

public sealed class GetProductHandler(IProductRepository products)
{
    private readonly IProductRepository _products = products;

    public async Task<ProductDto> HandleAsync(GetProductQuery query, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(query.ProductId, ct)
            ?? throw DomainErrors.NotFound($"Producto {query.ProductId}");

        return Map(product);
    }

    private static ProductDto Map(Product product)
        => new(product.Id, product.Name, product.Price.Amount, product.Price.Currency);
}


public sealed record ListProductsQuery;

public sealed class ListProductsHandler(IProductRepository products)
{
    private readonly IProductRepository _products = products;

    public async Task<IReadOnlyList<ProductDto>> HandleAsync(ListProductsQuery query, CancellationToken ct = default)
    {
        var all = await _products.GetAllAsync(ct);
        return all.Select(Map).ToList();
    }

    private static ProductDto Map(Product product)
        => new(product.Id, product.Name, product.Price.Amount, product.Price.Currency);
}
