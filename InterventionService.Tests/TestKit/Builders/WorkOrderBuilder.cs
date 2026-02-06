using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkOrders;

namespace InterventionService.Tests.TestKit.Builders;

public sealed class WorkOrderBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _orgId = Guid.NewGuid();
    private WorkOrderKind _kind = WorkOrderKind.Workshop;
    private DateTime _scheduledAt = DateTime.UtcNow;
    private string _currency = "EUR";

    private Guid? _vehicleId = Guid.NewGuid();
    private Guid? _clientId = null;
    private Guid? _definitionId = Guid.NewGuid();
    private Guid? _technicianId = null;
    private string? _notes = null;

    public WorkOrderBuilder WithId(Guid id) { _id = id; return this; }
    public WorkOrderBuilder WithOrg(Guid orgId) { _orgId = orgId; return this; }
    public WorkOrderBuilder AsWorkshop(Guid? vehicleId = null, Guid? definitionId = null)
    {
        _kind = WorkOrderKind.Workshop;
        _vehicleId = vehicleId ?? Guid.NewGuid();
        _definitionId = definitionId ?? Guid.NewGuid();
        _clientId = null;
        return this;
    }

    public WorkOrderBuilder AsCounterSale(Guid? clientId = null)
    {
        _kind = WorkOrderKind.CounterSale;
        _clientId = clientId ?? Guid.NewGuid();
        _vehicleId = null;
        _definitionId = null;
        return this;
    }

    public WorkOrderBuilder WithNotes(string? notes) { _notes = notes; return this; }

    public WorkOrder Build() => new(
        id: _id,
        organizationId: _orgId,
        kind: _kind,
        scheduledAt: _scheduledAt,
        currency: _currency,
        vehicleId: _vehicleId,
        clientId: _clientId,
        definitionId: _definitionId,
        technicianId: _technicianId,
        notes: _notes
    );
}
