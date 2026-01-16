using OrganizationService.Domain.Enums;

namespace OrganizationService.Api.Contracts.Requests;

public record InviteMemberRequest(
    Guid ActorUserId,
    Guid UserId,
    MemberRole Role
);
