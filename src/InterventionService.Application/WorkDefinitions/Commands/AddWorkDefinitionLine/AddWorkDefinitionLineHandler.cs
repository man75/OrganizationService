using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.WorkDefinitions;
using InterventionService.Domain.Exceptions;
using MediatR;

namespace InterventionService.Application.WorkDefinitions.Commands.AddWorkDefinitionLine;

public sealed class AddWorkDefinitionLineHandler
    : IRequestHandler<AddWorkDefinitionLineCommand, Result<WorkDefinitionDto>>
{
    private readonly IUserContext _current;
    private readonly IWorkDefinitionRepository _repo;
    private readonly IUnitOfWork _uow;

    public AddWorkDefinitionLineHandler(
        IUserContext current,
        IWorkDefinitionRepository repo,
        IUnitOfWork uow)
    {
        _current = current;
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkDefinitionDto>> Handle(
        AddWorkDefinitionLineCommand request,
        CancellationToken ct)
    {
        var orgId = _current.OrganizationId;

        var entity = await _repo.GetByIdAsync(request.WorkDefinitionId, ct);
        if (entity is null || entity.OrganizationId != orgId)
            return Result<WorkDefinitionDto>.Failure("Work definition not found.");

        try
        {
            entity.AddLine(
                type: request.Type,
                label: request.Label,
                quantity: request.Quantity,
                productId: request.ProductId,
                unitPriceExclTax: request.UnitPriceExclTax,
                vatRate: request.VatRate,
                sortOrder: request.SortOrder
            );
        }
        catch (DomainException ex)
        {
            return Result<WorkDefinitionDto>.Failure(ex.Message);
        }

        await _uow.SaveChangesAsync(ct);

        return Result<WorkDefinitionDto>.Success(
            WorkDefinitionMapper.ToDto(entity)
        );
    }
}