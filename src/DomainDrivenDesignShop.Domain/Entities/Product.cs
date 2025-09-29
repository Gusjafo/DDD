using DomainDrivenDesignShop.Domain.ValueObjects;

namespace DomainDrivenDesignShop.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Money Price { get; private set; }


    private Product() {}


    private Product(Guid id, string name, Money price)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("El nombre es requerido.");
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = name.Trim();
        Price = price;
    }


    public static Product Create(string name, Money price) => new(Guid.NewGuid(), name, price);


    public void Update(string name, Money price)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("El nombre es requerido.");
        Name = name.Trim();
        Price = price ?? throw new ArgumentNullException(nameof(price));
    }
}