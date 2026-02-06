using InterventionService.Domain.Exceptions;
using InterventionService.Domain.WorkOrders;

namespace InterventionService.Tests.Domain.WorkOrders;

public class MoneyTests
{
    [Fact]
    public void Ctor_Should_Set_Amount_And_Currency()
    {
        var money = new Money(10, "eur");

        money.Amount.Should().Be(10);
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Ctor_Should_Throw_When_Amount_Negative()
    {
        Action act = () => new Money(-1, "EUR");

        act.Should().Throw<DomainException>();
    }
}
