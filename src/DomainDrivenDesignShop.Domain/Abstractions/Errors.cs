namespace DomainDrivenDesignShop.Domain.Abstractions;
public static class DomainErrors
{
    public static Exception NotFound(string what) => new InvalidOperationException($"{what} no encontrado.");
    public static Exception RuleViolation(string message) => new InvalidOperationException(message);
}