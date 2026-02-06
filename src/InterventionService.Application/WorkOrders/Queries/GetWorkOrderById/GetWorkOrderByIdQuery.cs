using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using MediatR;

namespace InterventionService.Application.WorkOrders.Queries.GetWorkOrderById;

public sealed record GetWorkOrderByIdQuery(Guid Id) : IRequest<Result<WorkOrderDto>>;
