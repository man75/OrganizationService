using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using MediatR;

namespace InterventionService.Application.WorkOrders.Commands.StartWorkOrder;

public sealed record StartWorkOrderCommand(Guid WorkOrderId) : IRequest<Result<WorkOrderDto>>;
