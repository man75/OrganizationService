using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkDefinitions;

namespace InterventionService.Application.DTOs;

public sealed record WorkDefinitionDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    InterventionType Type,
    WorkDefinitionStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
