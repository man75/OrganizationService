using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkOrders;

namespace InterventionService.Application.DTOs;

public sealed record WorkOrderLineDto(
    Guid Id,
    WorkOrderLineType Type,
    Guid? ProductId,
    string Label,
    decimal Quantity,
    decimal UnitPriceExclTax,
    decimal VatRate,
    int SortOrder
);
