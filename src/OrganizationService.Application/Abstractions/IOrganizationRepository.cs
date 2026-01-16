using OrganizationService.Domain.Organizations;

namespace OrganizationService.Application.Abstractions;

public interface IOrganizationRepository
{
    Task<Organization?> GetAsync(Guid id, CancellationToken ct);
    Task<bool> SiretExistsAsync(string siret, CancellationToken ct);
    void Add(Organization organization);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
