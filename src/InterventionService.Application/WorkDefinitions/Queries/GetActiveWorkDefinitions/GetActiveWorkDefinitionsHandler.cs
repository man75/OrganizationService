using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.WorkDefinitions;
using MediatR;

namespace InterventionService.Application.WorkDefinitions.Queries.GetActiveWorkDefinitions;

public sealed class GetActiveWorkDefinitionsHandler
    : IRequestHandler<GetActiveWorkDefinitionsQuery, Result<IReadOnlyList<WorkDefinitionDto>>>
{
    private readonly IUserContext _current;
    private readonly IWorkDefinitionRepository _repo;

    public GetActiveWorkDefinitionsHandler(IUserContext current, IWorkDefinitionRepository repo)
    {
        _current = current;
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<WorkDefinitionDto>>> Handle(GetActiveWorkDefinitionsQuery request, CancellationToken ct)
    {
        var defs = await _repo.GetActiveAsync(_current.OrganizationId, ct);

        var dto = defs.Select(WorkDefinitionMapper.ToDto).ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<WorkDefinitionDto>>.Success(dto);
    }
}
