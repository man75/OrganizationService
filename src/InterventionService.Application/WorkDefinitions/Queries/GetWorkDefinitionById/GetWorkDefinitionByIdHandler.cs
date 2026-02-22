using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.WorkDefinitions;
using MediatR;

namespace InterventionService.Application.WorkDefinitions.Queries.GetWorkDefinitionById;

public sealed class GetWorkDefinitionByIdHandler
    : IRequestHandler<GetWorkDefinitionByIdQuery, Result<WorkDefinitionDto>>
{
    private readonly IUserContext _current;
    private readonly IWorkDefinitionRepository _repo;

    public GetWorkDefinitionByIdHandler(IUserContext current, IWorkDefinitionRepository repo)
    {
        _current = current;
        _repo = repo;
    }

    public async Task<Result<WorkDefinitionDto>> Handle(GetWorkDefinitionByIdQuery request, CancellationToken ct)
    {
        var orgId = _current.OrganizationId;

        // ✅ important: charge aussi les lignes
        var entity = await _repo.GetByIdWithLinesAsync(request.Id, ct);
        if (entity is null || entity.OrganizationId != orgId)
            return Result<WorkDefinitionDto>.Failure("Work definition not found.");

        return Result<WorkDefinitionDto>.Success(WorkDefinitionMapper.ToDto(entity, includeLines: true));
    }
}