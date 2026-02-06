using InterventionService.Application.DTOs;
using InterventionService.Domain.WorkOrders;

namespace InterventionService.Application.WorkOrders;

internal static class WorkOrderMapper
{
    public static WorkOrderDto ToDto(WorkOrder wo)
        => new(
            wo.Id,
            wo.OrganizationId,
            wo.Kind,
            wo.VehicleId,
            wo.ClientId,
            wo.DefinitionId,
            wo.TechnicianId,
            wo.Status,
            wo.ScheduledAt,
            wo.StartedAt,
            wo.CompletedAt,
            wo.Currency,
            wo.Notes,
            wo.TotalExclTax().Amount,
            wo.TotalTax().Amount,
            wo.TotalInclTax().Amount,
            wo.Lines.Select(l => new WorkOrderLineDto(
                l.Id,
                l.Type,
                l.ProductId,
                l.Label,
                l.Quantity,
                l.UnitPriceExclTax.Amount,
                l.VatRate,
                l.SortOrder
            )).ToList()
        );
}
