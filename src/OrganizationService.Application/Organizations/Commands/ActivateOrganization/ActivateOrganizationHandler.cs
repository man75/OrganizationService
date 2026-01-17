using MediatR;
using OrganizationService.Application.Abstractions;
using OrganizationService.Application.Organizations.Commands.UpdateOrganization;
using OrganizationService.Domain.Exceptions;

namespace OrganizationService.Application.Organizations.Commands.ActivateOrganization;

public class ActivateOrganizationHandler : IRequestHandler<ActivateOrganizationCommand>
{
    private readonly IOrganizationRepository _repo;

    public ActivateOrganizationHandler(IOrganizationRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(ActivateOrganizationCommand cmd, CancellationToken ct)
    {
        var org = await _repo.GetAsync(cmd.Id, ct);
        if (org is null)
            throw new DomainException("Organization not found.");

        org.Activate();

        await _repo.SaveChangesAsync(ct);
    }
}
