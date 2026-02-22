using MediatR;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;

namespace InterventionService.Application.WorkOrders.Commands.AddProductLine;

public sealed record AddWorkOrderProductLineCommand(Guid WorkOrderId, Guid ProductId, decimal Quantity)
    : IRequest<Result<WorkOrderDto>>;