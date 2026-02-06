using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.WorkDefinitions.Queries.GetActiveWorkDefinitions;
using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkDefinitions;
using InterventionService.Tests.TestKit.Fakes;

namespace InterventionService.Tests.Application.WorkDefinitions;

public class GetActiveWorkDefinitionsHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Active_WorkDefinitions_For_Organization()
    {
        // Arrange
        var current = new FakeCurrentUser();
        var orgId = current.OrganizationId;

        var defs = new List<WorkDefinition>
        {
            new(
                Guid.NewGuid(),
                orgId,
                "Vidange moteur",
                InterventionType.Maintenance
            ),
            new(
                Guid.NewGuid(),
                orgId,
                "Diagnostic électronique",
                InterventionType.Diagnostic
            )
        };

        var repo = Substitute.For<IWorkDefinitionRepository>();
        repo.GetActiveAsync(orgId, Arg.Any<CancellationToken>())
            .Returns(defs);

        var handler = new GetActiveWorkDefinitionsHandler(current, repo);

        // Act
        var result = await handler.Handle(new GetActiveWorkDefinitionsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Count.Should().Be(2);

        result.Value!.Select(x => x.Name)
            .Should()
            .Contain(new[] { "Vidange moteur", "Diagnostic électronique" });

        await repo.Received(1)
            .GetActiveAsync(orgId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Definitions()
    {
        // Arrange
        var current = new FakeCurrentUser();
        var repo = Substitute.For<IWorkDefinitionRepository>();

        repo.GetActiveAsync(current.OrganizationId, Arg.Any<CancellationToken>())
            .Returns(new List<WorkDefinition>());

        var handler = new GetActiveWorkDefinitionsHandler(current, repo);

        // Act
        var result = await handler.Handle(new GetActiveWorkDefinitionsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().BeEmpty();
    }
}
