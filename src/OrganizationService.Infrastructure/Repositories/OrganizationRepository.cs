using Microsoft.EntityFrameworkCore;
using OrganizationService.Application.Abstractions;
using OrganizationService.Domain.Organizations;
using OrganizationService.Infrastructure.Persistence;

namespace OrganizationService.Infrastructure.Repositories;

public class OrganizationRepository(OrganizationDbContext db) : IOrganizationRepository
{
    public Task<Organization?> GetAsync(Guid id, CancellationToken ct) =>
        db.Organizations
          .Include(o => o.Members)
          .FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<bool> SiretExistsAsync(string siret, CancellationToken ct) =>
        db.Organizations.AnyAsync(o => EF.Property<string?>(o, "Siret") == siret, ct); // si Siret n’existe pas encore -> on gèrera après migration

    public void Add(Organization organization) => db.Organizations.Add(organization);

    public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
