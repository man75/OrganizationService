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
using InterventionService.Domain.Enums;

namespace InterventionService.Application.WorkOrders.Commands.UpdateWorkOrder;

public sealed class UpdateWorkOrderCommandHandler
    : IRequestHandler<UpdateWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IUserContext _current;
    private readonly IWorkOrderRepository _repo;
    private readonly IUnitOfWork _uow;

    public UpdateWorkOrderCommandHandler(IUserContext current, IWorkOrderRepository repo, IUnitOfWork uow)
    {
        _current = current;
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkOrderDto>> Handle(UpdateWorkOrderCommand request, CancellationToken ct)
    {
        var orgId = _current.OrganizationId;
        var db = (DbContext)_uow;

        var order = await _repo.GetByIdWithLinesAsync(request.WorkOrderId, ct);
        if (order is null || order.OrganizationId != orgId)
            return Result<WorkOrderDto>.Failure("WorkOrder not found.");

        // ✅ Bloquer si terminé/annulé (ticket 1)
        if (order.Status != WorkOrderStatus.Draft && order.Status != WorkOrderStatus.InProgress)
            return Result<WorkOrderDto>.Failure("WorkOrder cannot be modified in its current status.");

        var input = request.Input;

        // ✅ Update header
        order.UpdateSchedule(input.ScheduledAt);
        order.AssignTechnician(input.TechnicianId);
        order.UpdateNotes(input.Notes);

        // ✅ 1) marquer les anciennes lignes comme Deleted (important EF/backing field)
        var oldLines = order.Lines.ToList();
        foreach (var line in oldLines)
        {
            db.Entry(line).State = EntityState.Deleted;

            // Owned Money: sécurisation
            var moneyEntry = db.Entry(line).Reference(x => x.UnitPriceExclTax).TargetEntry;
            if (moneyEntry is not null)
                moneyEntry.State = EntityState.Deleted;
        }

        // ✅ 2) vider côté domaine
        order.ClearLines();

        // ✅ 3) re-add + forcer Added
        foreach (var l in (input.Lines ?? Array.Empty<UpdateWorkOrderLineInput>()).OrderBy(x => x.SortOrder))
        {
            if (l.Type == 0) return Result<WorkOrderDto>.Failure("Invalid line type (0).");
            if (string.IsNullOrWhiteSpace(l.Label)) return Result<WorkOrderDto>.Failure("Line label is required.");
            if (l.Quantity <= 0) return Result<WorkOrderDto>.Failure("Line quantity must be > 0.");
            if (l.UnitPriceExclTax < 0) return Result<WorkOrderDto>.Failure("Unit price cannot be negative.");
            if (l.VatRate < 0) return Result<WorkOrderDto>.Failure("VAT rate cannot be negative.");

            var beforeCount = order.Lines.Count;

            order.AddLine(
                type: ((WorkDefinitionLineType)l.Type).ToOrderLineType(),
                label: l.Label.Trim(),
                quantity: l.Quantity,
                unitPriceExclTax: l.UnitPriceExclTax,
                vatRate: l.VatRate,
                productId: l.ProductId,
                sortOrder: l.SortOrder
            );

            // ✅ EF state fix (comme ApplyDefinition)
            if (order.Lines.Count > beforeCount)
            {
                var newLine = order.Lines.Last();

                if (newLine.Id == Guid.Empty)
                    return Result<WorkOrderDto>.Failure("BUG: WorkOrderLine.Id is Guid.Empty (must be Guid.NewGuid()).");

                db.Entry(newLine).State = EntityState.Added;

                var moneyEntry = db.Entry(newLine).Reference(x => x.UnitPriceExclTax).TargetEntry;
                if (moneyEntry is not null)
                    moneyEntry.State = EntityState.Added;
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

            return Result<WorkOrderDto>.Failure($"UpdateWorkOrder failed at SAVE (concurrency). Entries: {details}");
        }
        catch (DbUpdateException ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            return Result<WorkOrderDto>.Failure($"Database error: {msg}");
        }
        catch (Exception ex)
        {
            return Result<WorkOrderDto>.Failure($"Save error: {ex.Message}");
        }
    }
}