using System;
using System.Collections.Generic;

namespace InterventionService.Api.Contracts.Requests;

public sealed record UpdateWorkOrderRequest(
    DateTime ScheduledAt,
    Guid? TechnicianId,
    string? Notes,
    IReadOnlyList<UpdateWorkOrderLineRequest> Lines
);

public sealed record UpdateWorkOrderLineRequest(
    int Type,
    string Label,
    decimal Quantity,
    decimal UnitPriceExclTax,
    decimal VatRate,
    Guid? ProductId,
    int SortOrder
);