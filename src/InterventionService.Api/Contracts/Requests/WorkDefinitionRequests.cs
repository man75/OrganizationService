using InterventionService.Domain.Enums;

namespace InterventionService.API.Contracts.WorkDefinitions;

public sealed record CreateWorkDefinitionRequest(
  string Name,
    InterventionType Type,
    int? EstimatedMinutes,
    string? Notes
);
