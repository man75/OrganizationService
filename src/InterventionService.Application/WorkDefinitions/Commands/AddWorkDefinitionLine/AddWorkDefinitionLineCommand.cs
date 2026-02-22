using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Domain.Enums;
using MediatR;

namespace InterventionService.Application.WorkDefinitions.Commands.AddWorkDefinitionLine;

public sealed record AddWorkDefinitionLineCommand(
    Guid WorkDefinitionId,
    WorkDefinitionLineType Type,
    string Label,
    decimal Quantity,
    Guid? ProductId,
    decimal? UnitPriceExclTax,
    decimal? VatRate,
    int SortOrder
) : IRequest<Result<WorkDefinitionDto>>;