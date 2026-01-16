using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrganizationService.Api.Contracts.Requests;
using OrganizationService.Application.Organizations.Commands.CreateOrganization;
using OrganizationService.Application.Organizations.Commands.InviteMember;
using OrganizationService.Application.Organizations.Commands.UpdateOrganization;
using OrganizationService.Application.Organizations.Queries.GetOrganization;

namespace OrganizationService.Api.Controllers;

[ApiController]
[Route("api/organizations")]
public class OrganizationsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrganizationRequest req, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateOrganizationCommand(
            req.CreatorUserId,
            req.Name,
            req.Type,
            req.Siret
        ), ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    UpdateOrganizationRequest req,
    CancellationToken ct)
    {
        // plus tard : UserId depuis le token
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");


        await mediator.Send(new UpdateOrganizationCommand(
            id,
            userId,
            req.Name,
            req.Type,
            req.Siret
        ), ct);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrganizationQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{organizationId:guid}/members/invite")]
    public async Task<IActionResult> Invite(Guid organizationId, InviteMemberRequest req, CancellationToken ct)
    {
        await mediator.Send(new InviteMemberCommand(
            req.ActorUserId,
            organizationId,
            req.UserId,
            req.Role
        ), ct);

        return NoContent();
    }
}
