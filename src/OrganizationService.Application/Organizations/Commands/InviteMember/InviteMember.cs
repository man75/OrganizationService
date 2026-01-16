using MediatR;
using OrganizationService.Domain.Enums;

namespace OrganizationService.Application.Organizations.Commands.InviteMember;

public record InviteMemberCommand(
    Guid ActorUserId,
    Guid OrganizationId,
    Guid UserId,
    MemberRole Role
) : IRequest;
