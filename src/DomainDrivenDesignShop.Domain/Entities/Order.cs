using DomainDrivenDesignShop.Domain.Abstractions;
using DomainDrivenDesignShop.Domain.ValueObjects;

namespace DomainDrivenDesignShop.Domain.Entities;
public sealed class Order
{
    public Guid Id { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string Currency { get; private set; }


    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();


    private Order() { /* EF */ }


    private Order(string currency)
    {
        Id = Guid.NewGuid();
        CreatedAtUtc = DateTime.UtcNow;
        Currency = string.IsNullOrWhiteSpace(currency) || currency.Length != 3
        ? throw new ArgumentException("La divisa del pedido debe ser ISO de 3 letras.")
        : currency.ToUpperInvariant();
    }


    public static Order Create(string currency) => new(currency);


    public OrderLine AddProduct(Product product, int quantity)
    {
        if (product is null) throw DomainErrors.RuleViolation("Producto inválido.");
        if (product.Price.Currency != Currency)
            throw DomainErrors.RuleViolation("La divisa del producto no coincide con la del pedido.");


        var line = new OrderLine(Id, product.Id, quantity, product.Price);
        _lines.Add(line);
        return line;
    }


    public Money Total()
    {
        var total = Money.From(0, Currency);
        foreach (var l in _lines)
        {
            total = total.Add(l.Subtotal());
        }
        return total;
    }
}