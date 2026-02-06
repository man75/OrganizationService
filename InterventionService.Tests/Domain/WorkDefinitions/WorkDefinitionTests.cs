using InterventionService.Domain.Enums;
using InterventionService.Domain.Exceptions;
using InterventionService.Domain.WorkDefinitions;

namespace InterventionService.Tests.Domain.WorkDefinitions;

public class WorkDefinitionTests
{
    [Fact]
    public void Ctor_Should_Create_Active_Definition()
    {
        var def = new WorkDefinition(Guid.NewGuid(), Guid.NewGuid(), "Vidange", InterventionType.Maintenance);

        def.Status.Should().Be(WorkDefinitionStatus.Active);
    }

    [Fact]
    public void Rename_Should_Trim_Name()
    {
        var def = new WorkDefinition(Guid.NewGuid(), Guid.NewGuid(), "Vidange", InterventionType.Maintenance);

        def.Rename("  Révision  ");

        def.Name.Should().Be("Révision");
    }

    [Fact]
    public void Ctor_Should_Throw_When_Name_Invalid()
    {
        Action act = () =>
            new WorkDefinition(Guid.NewGuid(), Guid.NewGuid(), " ", InterventionType.Maintenance);

        act.Should().Throw<DomainException>();
    }
}
