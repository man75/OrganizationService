using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.WorkDefinitions.Commands.CreateWorkDefinition;
using InterventionService.Domain.Enums;
using InterventionService.Tests.TestKit.Fakes;

namespace InterventionService.Tests.Application.WorkDefinitions;

public class CreateWorkDefinitionHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Definition()
    {
        var current = new FakeCurrentUser();

        var repo = Substitute.For<IWorkDefinitionRepository>();
        repo.ExistsByNameAsync(current.OrganizationId, "Vidange", Arg.Any<CancellationToken>())
            .Returns(false);

        var uow = Substitute.For<IUnitOfWork>();

        var handler = new CreateWorkDefinitionHandler(current, repo, uow);

        var result = await handler.Handle(
            new CreateWorkDefinitionCommand("Vidange", InterventionType.Maintenance),
            default);

        result.IsSuccess.Should().BeTrue();
        await repo.Received(1).AddAsync(Arg.Any<InterventionService.Domain.WorkDefinitions.WorkDefinition>(), Arg.Any<CancellationToken>());
        await uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
