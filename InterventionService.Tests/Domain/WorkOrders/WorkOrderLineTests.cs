using InterventionService.Domain.Enums;
using InterventionService.Domain.Exceptions;
using InterventionService.Domain.WorkOrders;

namespace InterventionService.Tests.Domain.WorkOrders;

public class WorkOrderLineTests
{
    [Fact]
    public void Part_Line_Should_Require_ProductId()
    {
        Action act = () =>
            new WorkOrderLine(
                Guid.NewGuid(),
                WorkOrderLineType.Part,
                "Plaquette",
                1,
                new Money(50, "EUR"),
                0.2m,
                null);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Total_Should_Be_Computed()
    {
        var line = new WorkOrderLine(
            Guid.NewGuid(),
            WorkOrderLineType.Labor,
            "MO",
            2,
            new Money(50, "EUR"),
            0.2m);

        line.TotalInclTax().Amount.Should().Be(120);
    }
}
