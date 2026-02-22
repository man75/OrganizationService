using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkDefinitions;

namespace InterventionService.Application.DTOs;

public sealed record WorkDefinitionDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    InterventionType Type,
    WorkDefinitionStatus Status,
    int? EstimatedMinutes,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyCollection<WorkDefinitionLineDto> Lines
);
public sealed record WorkDefinitionLineDto(
    Guid Id,
    WorkDefinitionLineType Type,
    string Label,
    decimal Quantity,
    Guid? ProductId,
    decimal? UnitPriceExclTax,
    decimal? VatRate,
    int SortOrder
);