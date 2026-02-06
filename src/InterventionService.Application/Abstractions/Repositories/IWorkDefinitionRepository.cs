using InterventionService.Domain.WorkDefinitions;

namespace InterventionService.Application.Abstractions.Repositories;

public interface IWorkDefinitionRepository
{
    Task AddAsync(WorkDefinition entity, CancellationToken ct);
    Task<WorkDefinition?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsByNameAsync(Guid organizationId, string name, CancellationToken ct);
    Task<IReadOnlyList<WorkDefinition>> GetActiveAsync(Guid organizationId, CancellationToken ct);
    Task<bool> ExistsActiveAsync(Guid organizationId, Guid workDefinitionId, CancellationToken ct);
}
