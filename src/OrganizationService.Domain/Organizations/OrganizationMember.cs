using OrganizationService.Domain.Enums;

namespace OrganizationService.Domain.Organizations;

public class OrganizationMember
{
    private OrganizationMember() { } // EF

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }

    // ID externe (issu du Auth service)
    public Guid UserId { get; private set; }

    public MemberRole Role { get; private set; }
    public MemberStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    internal static OrganizationMember CreateAdmin(Guid organizationId, Guid userId)
        => new()
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            UserId = userId,
            Role = MemberRole.OrgAdmin,
            Status = MemberStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

    internal static OrganizationMember Invite(Guid organizationId, Guid userId, MemberRole role)
        => new()
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            UserId = userId,
            Role = role,
            Status = MemberStatus.Invited,
            CreatedAt = DateTime.UtcNow
        };

    internal void Activate()
    {
        if (Status == MemberStatus.Disabled) return;
        Status = MemberStatus.Active;
    }

    internal void ChangeRole(MemberRole role)
    {
        Role = role;
    }

    internal void Disable()
    {
        Status = MemberStatus.Disabled;
    }
}
