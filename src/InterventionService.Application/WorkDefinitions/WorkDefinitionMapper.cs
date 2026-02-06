using InterventionService.Application.DTOs;
using InterventionService.Domain.WorkDefinitions;

namespace InterventionService.Application.WorkDefinitions;

internal static class WorkDefinitionMapper
{
    public static WorkDefinitionDto ToDto(WorkDefinition d)
        => new(
            d.Id,
            d.OrganizationId,
            d.Name,
            d.Type,
            d.Status,
            d.CreatedAt,
            d.UpdatedAt
        );
}
