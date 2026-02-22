using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkDefinitions;

namespace InterventionService.Tests.TestKit.Builders;

public sealed class WorkDefinitionBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _orgId = Guid.NewGuid();
    private string _name = "Vidange moteur";
    private InterventionType _type = InterventionType.Maintenance;

    public WorkDefinitionBuilder WithId(Guid id) { _id = id; return this; }
    public WorkDefinitionBuilder WithOrg(Guid orgId) { _orgId = orgId; return this; }
    public WorkDefinitionBuilder WithName(string name) { _name = name; return this; }
    public WorkDefinitionBuilder WithType(InterventionType type) { _type = type; return this; }

    public WorkDefinition Build() => new(_id, _orgId, _name, _type);
}
