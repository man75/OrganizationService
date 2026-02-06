using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Domain.WorkDefinitions;
using Microsoft.EntityFrameworkCore;

namespace InterventionService.Infrastructure.Persistence.Repositories;

public sealed class WorkDefinitionRepository : IWorkDefinitionRepository
{
    private readonly InterventionDbContext _db;

    public WorkDefinitionRepository(InterventionDbContext db) => _db = db;

    public async Task AddAsync(WorkDefinition entity, CancellationToken ct)
        => await _db.WorkDefinitions.AddAsync(entity, ct);

    public Task<WorkDefinition?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.WorkDefinitions.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ExistsByNameAsync(Guid organizationId, string name, CancellationToken ct)
        => _db.WorkDefinitions.AnyAsync(
            x => x.OrganizationId == organizationId && x.Name == name,
            ct
        );

    public async Task<IReadOnlyList<WorkDefinition>> GetActiveAsync(Guid organizationId, CancellationToken ct)
        => await _db.WorkDefinitions
            .Where(x => x.OrganizationId == organizationId && x.Status == WorkDefinitionStatus.Active)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public async Task<bool> ExistsActiveAsync(
    Guid organizationId,
    Guid workDefinitionId,
    CancellationToken ct)
    => await _db.WorkDefinitions.AnyAsync(
        x => x.OrganizationId == organizationId
          && x.Id == workDefinitionId
          && x.Status == WorkDefinitionStatus.Active,
        ct
    );
}
