namespace DomainDrivenDesignShop.Domain.ValueObjects;
public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;

    private Money() {}

    // Ctor privado para controlar invariantes
    private Money(decimal amount, string currency)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "El importe no puede ser negativo.");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new ArgumentException("La divisa debe ser un código ISO de 3 letras.", nameof(currency));


        Amount = decimal.Round(amount, 2);
        Currency = currency.ToUpperInvariant();
    }


    public static Money From(decimal amount, string currency) => new(amount, currency);


    public Money Multiply(int factor)
    {
        if (factor < 0) throw new ArgumentOutOfRangeException(nameof(factor));
        return new Money(Amount * factor, Currency);
    }


    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("No se pueden sumar cantidades con distinta divisa.");
        return new Money(Amount + other.Amount, Currency);
    }


    public bool Equals(Money? other) => other is not null && Amount == other.Amount && Currency == other.Currency;
    public override bool Equals(object? obj) => Equals(obj as Money);
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    public override string ToString() => $"{Amount:N2} {Currency}";
}