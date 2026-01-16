using MediatR;
using OrganizationService.Application.Abstractions;
using OrganizationService.Application.Organizations.Commands.UpdateOrganization;
using OrganizationService.Domain.Exceptions;

namespace OrganizationService.Application.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationHandler : IRequestHandler<UpdateOrganizationCommand>
{
    private readonly IOrganizationRepository _repo;

    public UpdateOrganizationHandler(IOrganizationRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(UpdateOrganizationCommand cmd, CancellationToken ct)
    {
        var org = await _repo.GetAsync(cmd.Id, ct);
        if (org is null)
            throw new DomainException("Organization not found.");

        org.Update(cmd.Name, cmd.Type, cmd.Siret);

        await _repo.SaveChangesAsync(ct);
    }
}
