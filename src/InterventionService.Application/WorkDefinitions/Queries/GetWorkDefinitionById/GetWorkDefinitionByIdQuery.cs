using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using MediatR;

namespace InterventionService.Application.WorkDefinitions.Queries.GetWorkDefinitionById;

public sealed record GetWorkDefinitionByIdQuery(Guid Id)
    : IRequest<Result<WorkDefinitionDto>>;