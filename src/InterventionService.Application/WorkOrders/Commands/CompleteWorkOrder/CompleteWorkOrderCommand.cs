using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using MediatR;

namespace InterventionService.Application.WorkOrders.Commands.CompleteWorkOrder;

public sealed record CompleteWorkOrderCommand(Guid WorkOrderId) : IRequest<Result<WorkOrderDto>>;
