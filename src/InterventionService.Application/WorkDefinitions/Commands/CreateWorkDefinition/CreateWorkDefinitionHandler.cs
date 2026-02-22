

  using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.WorkDefinitions;
using InterventionService.Domain.WorkDefinitions;
using MediatR;

namespace InterventionService.Application.WorkDefinitions.Commands.CreateWorkDefinition;

public sealed class CreateWorkDefinitionHandler
    : IRequestHandler<CreateWorkDefinitionCommand, Result<WorkDefinitionDto>>
{
    private readonly IUserContext _current;
    private readonly IWorkDefinitionRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateWorkDefinitionHandler(
        IUserContext current,
        IWorkDefinitionRepository repo,
        IUnitOfWork uow)
    {
        _current = current;
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkDefinitionDto>> Handle(
    CreateWorkDefinitionCommand request,
    CancellationToken ct)
    {
        var orgId = _current.OrganizationId;

        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
            return Result<WorkDefinitionDto>.Failure("Name is required.");

        var exists = await _repo.ExistsByNameAsync(orgId, name, ct);
        if (exists)
            return Result<WorkDefinitionDto>.Failure("A work definition with the same name already exists.");

        var entity = new WorkDefinition(
            id: Guid.NewGuid(),
            organizationId: orgId,
            name: name,
            type: request.Type
        );

        // 🔥 NOUVEAU
        entity.SetEstimatedMinutes(request.EstimatedMinutes);
        entity.SetNotes(request.Notes);

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<WorkDefinitionDto>.Success(
            WorkDefinitionMapper.ToDto(entity)
        );
    }

}
