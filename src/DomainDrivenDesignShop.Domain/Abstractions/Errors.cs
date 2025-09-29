namespace DomainDrivenDesignShop.Domain.Abstractions;
public static class DomainErrors
{
    public static Exception NotFound(string what) => new DomainNotFoundException($"{what} no encontrado.");
    public static Exception RuleViolation(string message) => new DomainRuleViolationException(message);
}


public sealed class DomainNotFoundException(string message) : InvalidOperationException(message);
public sealed class DomainRuleViolationException(string message) : InvalidOperationException(message);