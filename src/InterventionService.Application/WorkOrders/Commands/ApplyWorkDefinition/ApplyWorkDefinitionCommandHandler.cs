using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.Common.Extensions;

namespace InterventionService.Application.WorkOrders.Commands.ApplyWorkDefinition;

public sealed class ApplyWorkDefinitionCommandHandler
    : IRequestHandler<ApplyWorkDefinitionCommand, Result<WorkOrderDto>>
{
    private readonly IUserContext _current;
    private readonly IWorkOrderRepository _orderRepo;
    private readonly IWorkDefinitionRepository _defRepo;
    private readonly IStockGateway _stock;
    private readonly IUnitOfWork _uow;

    public ApplyWorkDefinitionCommandHandler(
        IUserContext current,
        IWorkOrderRepository orderRepo,
        IWorkDefinitionRepository defRepo,
        IStockGateway stock,
        IUnitOfWork uow)
    {
        _current = current;
        _orderRepo = orderRepo;
        _defRepo = defRepo;
        _stock = stock;
        _uow = uow;
    }

    public async Task<Result<WorkOrderDto>> Handle(ApplyWorkDefinitionCommand request, CancellationToken ct)
    {
        var orgId = _current.OrganizationId;
        var db = (DbContext)_uow; // ✅ InterventionDbContext

        var order = await _orderRepo.GetByIdWithLinesAsync(request.WorkOrderId, ct);
        if (order is null || order.OrganizationId != orgId)
            return Result<WorkOrderDto>.Failure("WorkOrder not found.");

        var def = await _defRepo.GetByIdWithLinesAsync(request.DefinitionId, ct);
        if (def is null)
            return Result<WorkOrderDto>.Failure("WorkDefinition not found.");

        foreach (var l in def.Lines.OrderBy(x => x.SortOrder))
        {
            if ((int)l.Type == 0)
                return Result<WorkOrderDto>.Failure("WorkDefinition contains invalid line type (0).");

            decimal unitPrice = 0m;
            decimal vatRate = l.VatRate ?? 20m;

            if (l.ProductId.HasValue)
            {
                var p = await _stock.GetProductPricingAsync(orgId, l.ProductId.Value, ct);
                if (p is not null)
                {
                    unitPrice = p.UnitPriceExclTax;
                    vatRate = p.VatRate;
                }
            }

            var beforeCount = order.Lines.Count;

            order.AddLine(
                type: l.Type.ToOrderLineType(),
                label: l.Label,
                quantity: l.Quantity,
                unitPriceExclTax: unitPrice,
                vatRate: vatRate,
                productId: l.ProductId,
                sortOrder: l.SortOrder
            );

            // ✅ on récupère la ligne fraîchement ajoutée (si AddLine ajoute 1 élément)
            if (order.Lines.Count > beforeCount)
            {
                var newLine = order.Lines.Last();

                if (newLine.Id == Guid.Empty)
                    return Result<WorkOrderDto>.Failure("BUG: WorkOrderLine.Id is Guid.Empty (must be Guid.NewGuid()).");

                // ✅ FIX : EF ne doit jamais considérer cette ligne comme existante
                db.Entry(newLine).State = EntityState.Added;

                // Owned Money : souvent pas nécessaire, mais on sécurise
                db.Entry(newLine).Reference(x => x.UnitPriceExclTax).TargetEntry!.State = EntityState.Added;
            }
        }

        try
        {
            await _uow.SaveChangesAsync(ct);
            return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(order));
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var details = string.Join(" | ", ex.Entries.Select(e =>
            {
                var keys = string.Join(",", e.Properties
                    .Where(p => p.Metadata.IsPrimaryKey())
                    .Select(p => $"{p.Metadata.Name}={p.CurrentValue}"));
                return $"{e.Metadata.Name} State={e.State} Keys=[{keys}]";
            }));

            return Result<WorkOrderDto>.Failure($"Concurrency error at SAVE. Entries: {details}");
        }
        catch (DbUpdateException ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            return Result<WorkOrderDto>.Failure($"Erreur base de données : {msg}");
        }
    }
}