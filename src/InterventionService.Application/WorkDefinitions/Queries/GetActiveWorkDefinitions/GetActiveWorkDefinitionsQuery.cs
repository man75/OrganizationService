using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using MediatR;

namespace InterventionService.Application.WorkDefinitions.Queries.GetActiveWorkDefinitions;

public sealed record GetActiveWorkDefinitionsQuery()
    : IRequest<Result<IReadOnlyList<WorkDefinitionDto>>>;
