using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using MediatR;

namespace InterventionService.Application.WorkOrders.Commands.CreateCounterSale;

public sealed record CreateCounterSaleCommand(
    Guid ClientId,
    DateTime ScheduledAt,
    string? Notes
) : IRequest<Result<WorkOrderDto>>;
