using MediatR;
using OrganizationService.Application.Abstractions;
using OrganizationService.Application.Organizations.Commands.UpdateOrganization;
using OrganizationService.Domain.Exceptions;

namespace OrganizationService.Application.Organizations.Commands.DeleteOrganization;

public class SuspendOrganizationHandler : IRequestHandler<SuspendOrganizationCommand>
{
    private readonly IOrganizationRepository _repo;

    public SuspendOrganizationHandler(IOrganizationRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(SuspendOrganizationCommand cmd, CancellationToken ct)
    {
        var org = await _repo.GetAsync(cmd.Id, ct);
        if (org is null)
            throw new DomainException("Organization not found.");

        org.Suspend();

        await _repo.SaveChangesAsync(ct);
    }
}
