using MediatR;
using OrganizationService.Application.Abstractions;
using OrganizationService.Domain.Exceptions;

namespace OrganizationService.Application.Organizations.Commands.InviteMember;

public class InviteMemberHandler(IOrganizationRepository repo)
    : IRequestHandler<InviteMemberCommand>
{
    public async Task Handle(InviteMemberCommand cmd, CancellationToken ct)
    {
        var org = await repo.GetAsync(cmd.OrganizationId, ct);
        if (org is null) throw new DomainException("Organization not found.");

        org.InviteMember(cmd.ActorUserId, cmd.UserId, cmd.Role);

        await repo.SaveChangesAsync(ct);
    }
}
