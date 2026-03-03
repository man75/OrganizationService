namespace InterventionService.Application.WorkOrders.Commands.UpdateWorkOrder;

public sealed record UpdateWorkOrderInput(
    DateTime ScheduledAt,
    Guid? TechnicianId,
    string? Notes,
    IReadOnlyList<UpdateWorkOrderLineInput> Lines
);

public sealed record UpdateWorkOrderLineInput(
    int Type,
    string Label,
    decimal Quantity,
    decimal UnitPriceExclTax,
    decimal VatRate,
    Guid? ProductId,
    int SortOrder
);