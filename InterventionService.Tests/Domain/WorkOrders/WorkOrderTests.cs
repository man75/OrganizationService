using InterventionService.Domain.Enums;
using InterventionService.Domain.Exceptions;
using InterventionService.Domain.WorkOrders;
using InterventionService.Tests.TestKit.Builders;

namespace InterventionService.Tests.Domain.WorkOrders;

public class WorkOrderTests
{
    [Fact]
    public void Workshop_Should_Require_Vehicle_And_Definition()
    {
        Action act = () =>
            new WorkOrder(
                Guid.NewGuid(),
                Guid.NewGuid(),
                WorkOrderKind.Workshop,
                DateTime.UtcNow,
                "EUR",
                null,
                null,
                null);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Start_Should_Move_To_InProgress()
    {
        var wo = new WorkOrderBuilder().AsWorkshop().Build();

        wo.Start();

        wo.Status.Should().Be(WorkOrderStatus.InProgress);
    }
}
