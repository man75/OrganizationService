using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Application.DTOs;

namespace InterventionService.Application.WorkDefinitions;

internal static class WorkDefinitionMapper
{
    public static WorkDefinitionDto ToDto(WorkDefinition d, bool includeLines)
        => new(
            d.Id,
            d.OrganizationId,
            d.Name,
            d.Type,
            d.Status,
            d.EstimatedMinutes,
            d.Notes,
            d.CreatedAt,
            d.UpdatedAt,
            includeLines
                ? d.Lines
                    .OrderBy(x => x.SortOrder)
                    .Select(l => new WorkDefinitionLineDto(
                        l.Id,
                        l.Type,
                        l.Label,
                        l.Quantity,
                        l.ProductId,
                        l.UnitPriceExclTax,
                        l.VatRate,
                        l.SortOrder
                    ))
                    .ToList()
                : Array.Empty<WorkDefinitionLineDto>()
        );

    // ✅ compat avec ton code existant (sans lignes)
    public static WorkDefinitionDto ToDto(WorkDefinition d)
        => ToDto(d, includeLines: false);
}