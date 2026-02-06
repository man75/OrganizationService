using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkOrders;
using MediatR;

namespace InterventionService.Application.WorkOrders.Commands.AddLine;

public sealed record AddLineCommand(
    Guid WorkOrderId,
    WorkOrderLineType Type,
    string Label,
    decimal Quantity,
    decimal UnitPriceExclTax,
    decimal VatRate,
    Guid? ProductId,
    int SortOrder
) : IRequest<Result<WorkOrderDto>>;
