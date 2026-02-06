using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkOrders;

namespace InterventionService.API.Contracts.WorkOrders;

public sealed record CreateWorkshopWorkOrderRequest(
    Guid VehicleId,
    Guid DefinitionId,
    DateTime ScheduledAt,
    Guid? TechnicianId,
    string? Notes
);

public sealed record CreateCounterSaleRequest(
    Guid ClientId,
    DateTime ScheduledAt,
    string? Notes
);

public sealed record AddLineRequest(
    WorkOrderLineType Type,
    string Label,
    decimal Quantity,
    decimal UnitPriceExclTax,
    decimal VatRate,
    Guid? ProductId,
    int SortOrder
);

public sealed record CancelWorkOrderRequest(string? Reason);
