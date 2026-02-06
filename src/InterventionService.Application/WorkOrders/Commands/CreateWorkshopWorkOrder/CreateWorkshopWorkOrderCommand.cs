using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using MediatR;

namespace InterventionService.Application.WorkOrders.Commands.CreateWorkshopWorkOrder;

public sealed record CreateWorkshopWorkOrderCommand(
    Guid VehicleId,
    Guid DefinitionId,
    DateTime ScheduledAt,
    Guid? TechnicianId,
    string? Notes
) : IRequest<Result<WorkOrderDto>>;
