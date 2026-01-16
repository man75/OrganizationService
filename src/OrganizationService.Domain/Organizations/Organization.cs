using OrganizationService.Domain.Enums;
using OrganizationService.Domain.Exceptions;

namespace OrganizationService.Domain.Organizations;

public class Organization
{
    private readonly List<OrganizationMember> _members = new();

    private Organization() { } // EF

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Siret { get; private set; }
    public OrganizationType Type { get; private set; }
    public OrganizationStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<OrganizationMember> Members => _members.AsReadOnly();

    public Organization(Guid id, string name, string siret, OrganizationType type, Guid creatorUserId)
    {
        if (id == Guid.Empty) throw new DomainException("Organization id is required.");
        if (creatorUserId == Guid.Empty) throw new DomainException("Creator user id is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Organization name is required.");

        Id = id;
        Name = name.Trim();
        Type = type;
        Status = OrganizationStatus.Active;
        Siret = string.IsNullOrWhiteSpace(siret) ? null : siret.Trim();

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // ✅ règle métier : une org a toujours au moins 1 admin
        _members.Add(OrganizationMember.CreateAdmin(Id, creatorUserId));
    }
    public void Update(string name, OrganizationType type, string? siret)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required.");

        Name = name.Trim();
        Type = type;
        Siret = string.IsNullOrWhiteSpace(siret) ? null : siret.Trim();

        UpdatedAt = DateTime.UtcNow;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Organization name is required.");
        Name = name.Trim();
        Touch();
    }

    public void Suspend()
    {
        Status = OrganizationStatus.Suspended;
        Touch();
    }

    public void Activate()
    {
        Status = OrganizationStatus.Active;
        Touch();
    }

    public void InviteMember(Guid actorUserId, Guid userId, MemberRole role)
    {
        EnsureAdmin(actorUserId);

        if (userId == Guid.Empty) throw new DomainException("UserId is required.");
        if (_members.Any(m => m.UserId == userId))
            throw new DomainException("User is already a member.");

        _members.Add(OrganizationMember.Invite(Id, userId, role));
        Touch();
    }

    public void ChangeMemberRole(Guid actorUserId, Guid userId, MemberRole newRole)
    {
        EnsureAdmin(actorUserId);

        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member is null) throw new DomainException("Member not found.");

        // règle métier : on garde au moins 1 admin
        if (member.Role == MemberRole.OrgAdmin && newRole != MemberRole.OrgAdmin)
        {
            var admins = _members.Count(m => m.Role == MemberRole.OrgAdmin && m.Status != MemberStatus.Disabled);
            if (admins <= 1) throw new DomainException("Organization must have at least one admin.");
        }

        member.ChangeRole(newRole);
        Touch();
    }

    public void DisableMember(Guid actorUserId, Guid userId)
    {
        EnsureAdmin(actorUserId);

        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member is null) throw new DomainException("Member not found.");

        // règle métier : on ne peut pas désactiver le dernier admin
        if (member.Role == MemberRole.OrgAdmin)
        {
            var admins = _members.Count(m => m.Role == MemberRole.OrgAdmin && m.Status != MemberStatus.Disabled);
            if (admins <= 1) throw new DomainException("Cannot disable the last admin.");
        }

        member.Disable();
        Touch();
    }

    private void EnsureAdmin(Guid actorUserId)
    {
        var actor = _members.FirstOrDefault(m => m.UserId == actorUserId && m.Status == MemberStatus.Active);
        if (actor is null || actor.Role != MemberRole.OrgAdmin)
            throw new DomainException("Only organization admin can perform this action.");
    }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
