using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;

using MediatR;


namespace InterventionService.Application.WorkDefinitions.Commands.AddWorkDefinitionLines;

public sealed class AddWorkDefinitionLinesCommandHandler
    : IRequestHandler<AddWorkDefinitionLinesCommand, Result<WorkDefinitionDto>>
{
    private readonly IUserContext _current;
    private readonly IWorkDefinitionRepository _repo;
    private readonly IUnitOfWork _uow;

    public AddWorkDefinitionLinesCommandHandler(
        IUserContext current,
        IWorkDefinitionRepository repo,
        IUnitOfWork uow)
    {
        _current = current;
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkDefinitionDto>> Handle(AddWorkDefinitionLinesCommand request, CancellationToken ct)
    {
        var orgId = _current.OrganizationId;

        // ✅ charge avec lignes
        var def = await _repo.GetByIdAsync(request.WorkDefinitionId, ct);

        if (def is null || def.OrganizationId != orgId)
            return Result<WorkDefinitionDto>.Failure("Work definition not found.");

        // ✅ 1) supprimer toutes les lignes existantes
        def.ClearLines(); // méthode domaine (ci-dessous)

        // ✅ 2) ajouter celles reçues (option: dédoublonner côté back au cas où)
        var items = request.Lines ?? new List<AddWorkDefinitionLineItem>();

        // (optionnel mais conseillé) : dédoublonnage par ProductId+Type+Label
        var normalized = items
            .Where(i => !string.IsNullOrWhiteSpace(i.Label))
            .GroupBy(i => new
            {
                i.Type,
                ProductId = i.ProductId ?? Guid.Empty,
                Label = i.Label.Trim().ToUpperInvariant()
            })
            .Select(g =>
            {
                var first = g.First();
                return new AddWorkDefinitionLineItem(
                    first.Type,
                    first.Label.Trim(),
                    g.Sum(x => x.Quantity),         // ✅ qty cumulée si doublons dans la requête
                    first.ProductId,
                    first.UnitPriceExclTax,
                    first.VatRate,
                    first.SortOrder
                );
            })
            .OrderBy(x => x.SortOrder)
            .ToList();

        foreach (var i in normalized)
        {
            def.AddLine(
                type: i.Type,
                label: i.Label,
                quantity: i.Quantity,
                productId: i.ProductId,
                unitPriceExclTax: i.UnitPriceExclTax,
                vatRate: i.VatRate,
                sortOrder: i.SortOrder
            );
        }

        await _uow.SaveChangesAsync(ct);

        return Result<WorkDefinitionDto>.Success(WorkDefinitionMapper.ToDto(def, includeLines: true));
    }
}