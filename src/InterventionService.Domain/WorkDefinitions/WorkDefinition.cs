using InterventionService.Domain.Enums;
using InterventionService.Domain.Exceptions;

namespace InterventionService.Domain.WorkDefinitions;

/// <summary>
/// Type d’intervention / prestation catalogue
/// Ex : Vidange, Révision, Diagnostic
/// </summary>
public class WorkDefinition
{
    private WorkDefinition() { } // EF Core

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }

    public string Name { get; private set; } = default!;
    public InterventionType Type { get; private set; }

    public WorkDefinitionStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // --------------------
    // Constructor
    // --------------------
    public WorkDefinition(
        Guid id,
        Guid organizationId,
        string name,
        InterventionType type)
    {
        if (id == Guid.Empty)
            throw new DomainException("WorkDefinition id is required.");

        if (organizationId == Guid.Empty)
            throw new DomainException("OrganizationId is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("WorkDefinition name is required.");

        Id = id;
        OrganizationId = organizationId;
        Name = name.Trim();
        Type = type;

        Status = WorkDefinitionStatus.Active;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // --------------------
    // Business rules
    // --------------------
    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required.");

        Name = name.Trim();
        Touch();
    }

    public void ChangeType(InterventionType type)
    {
        Type = type;
        Touch();
    }

    public void Activate()
    {
        if (Status == WorkDefinitionStatus.Active) return;
        Status = WorkDefinitionStatus.Active;
        Touch();
    }

    public void Deactivate()
    {
        if (Status == WorkDefinitionStatus.Inactive) return;
        Status = WorkDefinitionStatus.Inactive;
        Touch();
    }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
