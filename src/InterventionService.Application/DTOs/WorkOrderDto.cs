using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkOrders;

namespace InterventionService.Application.DTOs;

public sealed record WorkOrderDto(
    Guid Id,
    Guid OrganizationId,
    WorkOrderKind Kind,
    Guid? VehicleId,
    Guid? ClientId,
    Guid? DefinitionId,
    Guid? TechnicianId,
    WorkOrderStatus Status,
    DateTime ScheduledAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string Currency,
    string? Notes,
    decimal TotalExclTax,
    decimal TotalTax,
    decimal TotalInclTax,
    IReadOnlyList<WorkOrderLineDto> Lines
);
