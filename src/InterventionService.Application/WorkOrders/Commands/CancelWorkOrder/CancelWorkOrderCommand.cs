using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using MediatR;

namespace InterventionService.Application.WorkOrders.Commands.CancelWorkOrder;

public sealed record CancelWorkOrderCommand(Guid WorkOrderId, string? Reason) : IRequest<Result<WorkOrderDto>>;
