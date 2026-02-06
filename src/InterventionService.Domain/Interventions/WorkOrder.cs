using InterventionService.Domain.Enums;
using InterventionService.Domain.Exceptions;

namespace InterventionService.Domain.WorkOrders;

public class WorkOrder
{
    private readonly List<WorkOrderLine> _lines = new();
    private WorkOrder() { } // EF

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }

    public WorkOrderKind Kind { get; private set; }

    // Workshop
    public Guid? VehicleId { get; private set; }

    // CounterSale (obligatoire)
    public Guid? ClientId { get; private set; }

    // ✅ Type d’intervention (catalogue) -> WorkDefinition.Id
    public Guid? DefinitionId { get; private set; }

    public Guid? TechnicianId { get; private set; }

    public WorkOrderStatus Status { get; private set; }

    public DateTime ScheduledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public string Currency { get; private set; } = "EUR";
    public string? Notes { get; private set; }

    public IReadOnlyCollection<WorkOrderLine> Lines => _lines.AsReadOnly();

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public WorkOrder(
        Guid id,
        Guid organizationId,
        WorkOrderKind kind,
        DateTime scheduledAt,
        string currency,
        Guid? vehicleId = null,
        Guid? clientId = null,
        Guid? definitionId = null,
        Guid? technicianId = null,
        string? notes = null)
    {
        if (id == Guid.Empty) throw new DomainException("WorkOrder id is required.");
        if (organizationId == Guid.Empty) throw new DomainException("OrganizationId is required.");
        if (scheduledAt == default) throw new DomainException("ScheduledAt is required.");
        if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency is required.");

        Id = id;
        OrganizationId = organizationId;

        Kind = kind;
        ScheduledAt = scheduledAt;
        Currency = currency.Trim().ToUpperInvariant();

        VehicleId = vehicleId;
        ClientId = clientId;
        DefinitionId = definitionId;
        TechnicianId = technicianId;

        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();

        Status = WorkOrderStatus.Planned;

        ValidateRules();

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private void ValidateRules()
    {
        if (Kind == WorkOrderKind.Workshop)
        {
            if (!VehicleId.HasValue || VehicleId.Value == Guid.Empty)
                throw new DomainException("VehicleId is required for workshop work orders.");

            if (!DefinitionId.HasValue || DefinitionId.Value == Guid.Empty)
                throw new DomainException("DefinitionId is required for workshop work orders.");
        }

        if (Kind == WorkOrderKind.CounterSale)
        {
            if (!ClientId.HasValue || ClientId.Value == Guid.Empty)
                throw new DomainException("ClientId is required for counter sales.");

            if (VehicleId.HasValue)
                throw new DomainException("VehicleId must be null for counter sales.");

            if (DefinitionId.HasValue)
                throw new DomainException("DefinitionId must be null for counter sales.");
        }
    }

    // --------------------
    // Lines
    // --------------------
    public Guid AddLine(
        WorkOrderLineType type,
        string label,
        decimal quantity,
        decimal unitPriceExclTax,
        decimal vatRate,
        Guid? productId = null,
        int sortOrder = 0)
    {
        EnsureEditable();

        var line = new WorkOrderLine(
            id: Guid.NewGuid(),
            type: type,
            label: label,
            quantity: quantity,
            unitPriceExclTax: new Money(unitPriceExclTax, Currency),
            vatRate: vatRate,
            productId: productId,
            sortOrder: sortOrder
        );

        _lines.Add(line);
        Touch();
        return line.Id;
    }

    public void UpdateLine(
        Guid lineId,
        decimal quantity,
        decimal unitPriceExclTax,
        decimal vatRate,
        string? label = null)
    {
        EnsureEditable();

        var line = GetLine(lineId);

        if (!string.IsNullOrWhiteSpace(label))
            line.Rename(label);

        line.UpdatePricing(quantity, new Money(unitPriceExclTax, Currency), vatRate);
        Touch();
    }

    public void RemoveLine(Guid lineId)
    {
        EnsureEditable();

        var line = GetLine(lineId);
        _lines.Remove(line);
        Touch();
    }

    private WorkOrderLine GetLine(Guid lineId)
    {
        if (lineId == Guid.Empty) throw new DomainException("LineId is required.");

        var line = _lines.FirstOrDefault(x => x.Id == lineId);
        if (line is null) throw new DomainException("Line not found.");

        return line;
    }

    // --------------------
    // Lifecycle
    // --------------------
    public void AssignTechnician(Guid technicianId)
    {
        EnsureEditable();

        if (technicianId == Guid.Empty)
            throw new DomainException("TechnicianId is required.");

        TechnicianId = technicianId;
        Touch();
    }

    public void Reschedule(DateTime scheduledAt)
    {
        if (Status != WorkOrderStatus.Planned)
            throw new DomainException("Only planned work orders can be rescheduled.");

        if (scheduledAt == default)
            throw new DomainException("ScheduledAt is required.");

        ScheduledAt = scheduledAt;
        Touch();
    }

    public void Start()
    {
        if (Status != WorkOrderStatus.Planned)
            throw new DomainException("Only planned work orders can be started.");

        Status = WorkOrderStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        Touch();
    }

    public void Complete()
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new DomainException("Only in-progress work orders can be completed.");

        if (_lines.Count == 0)
            throw new DomainException("Cannot complete a work order with no lines.");

        Status = WorkOrderStatus.Done;
        CompletedAt = DateTime.UtcNow;
        Touch();
    }

    public void Cancel(string? reason = null)
    {
        if (Status == WorkOrderStatus.Done)
            throw new DomainException("A completed work order cannot be cancelled.");

        Status = WorkOrderStatus.Cancelled;

        if (!string.IsNullOrWhiteSpace(reason))
            Notes = reason.Trim();

        Touch();
    }

    private void EnsureEditable()
    {
        if (Status is WorkOrderStatus.Done or WorkOrderStatus.Cancelled)
            throw new DomainException("Work order is not editable in this status.");
    }

    // --------------------
    // Totals
    // --------------------
    public Money TotalExclTax()
        => new(_lines.Sum(l => l.TotalExclTax().Amount), Currency);

    public Money TotalTax()
        => new(_lines.Sum(l => l.TotalTax().Amount), Currency);

    public Money TotalInclTax()
        => new(_lines.Sum(l => l.TotalInclTax().Amount), Currency);

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
