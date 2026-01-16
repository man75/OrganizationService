using MediatR;
using OrganizationService.Application.Abstractions;
using OrganizationService.Domain.Enums;
using OrganizationService.Domain.Organizations;
using OrganizationService.Domain.Exceptions;

namespace OrganizationService.Application.Organizations.Commands.CreateOrganization;

public class CreateOrganizationHandler(IOrganizationRepository repo)
    : IRequestHandler<CreateOrganizationCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrganizationCommand cmd, CancellationToken ct)
    {
        var id = Guid.NewGuid();

        // SIRET : validation simple ici (et le Domain refuse si requis)
        string? siret = string.IsNullOrWhiteSpace(cmd.Siret) ? null : cmd.Siret.Trim();

        if (siret is not null)
        {
            if (siret.Length != 14 || !siret.All(char.IsDigit))
                throw new DomainException("Invalid SIRET format (14 digits required).");

            if (await repo.SiretExistsAsync(siret, ct))
                throw new DomainException("SIRET already exists.");
        }

        // Pour rester MVP: on stocke SIRET en string (pas VO obligatoire tout de suite)
        var org = new Organization(
            id: id,
            name: cmd.Name,
            siret:cmd.Siret,
            type: cmd.Type,
            creatorUserId: cmd.CreatorUserId
        );

        // si tu as ajouté Siret au Domain, remplace par org.SetSiret(siret) (étape suivante)
        // sinon on le mettra au mapping plus tard.

        repo.Add(org);
        await repo.SaveChangesAsync(ct);

        return id;
    }
}
