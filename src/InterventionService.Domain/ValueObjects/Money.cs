using InterventionService.Domain.Exceptions;

namespace InterventionService.Domain.WorkOrders;

public sealed class Money
{
    private Money() { } // 🔴 indispensable pour EF

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = default!;

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required.");

        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }
}
