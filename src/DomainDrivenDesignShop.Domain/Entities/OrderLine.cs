using DomainDrivenDesignShop.Domain.ValueObjects;

namespace DomainDrivenDesignShop.Domain.Entities;
public sealed class OrderLine
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }


    private OrderLine() { /* EF */ }


    internal OrderLine(Guid orderId, Guid productId, int quantity, Money unitPrice)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser > 0.");
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }


    public Money Subtotal() => UnitPrice.Multiply(Quantity);
}