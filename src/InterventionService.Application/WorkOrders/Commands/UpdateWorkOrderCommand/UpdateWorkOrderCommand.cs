using MediatR;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;

namespace InterventionService.Application.WorkOrders.Commands.UpdateWorkOrder;

public sealed record UpdateWorkOrderCommand(Guid WorkOrderId, UpdateWorkOrderInput Input)
    : IRequest<Result<WorkOrderDto>>;