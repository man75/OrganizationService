using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Domain.Enums;
using MediatR;

namespace InterventionService.Application.WorkDefinitions.Commands.CreateWorkDefinition;

public sealed record CreateWorkDefinitionCommand(
    string Name,
    InterventionType Type
) : IRequest<Result<WorkDefinitionDto>>;
