using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkDefinitions;
using InterventionService.Domain.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InterventionService.Infrastructure.Persistence.Repositories;

public sealed class WorkOrderRepository : IWorkOrderRepository
{
    private readonly InterventionDbContext _db;

    public WorkOrderRepository(InterventionDbContext db) => _db = db;

    public async Task AddAsync(WorkOrder entity, CancellationToken ct)
        => await _db.WorkOrders.AddAsync(entity, ct);

    public async Task<WorkOrder?> GetByWithLignesIdAsync(
     Guid orgaId,
     Guid vehicleId,
     CancellationToken ct)
    {
        return await _db.WorkOrders
            .Include(x => x.Lines)
            .Where(x =>
                x.OrganizationId == orgaId &&
                x.VehicleId == vehicleId &&
                x.Kind == WorkOrderKind.Workshop &&
                x.Status == WorkOrderStatus.Draft)
            .OrderByDescending(x => x.UpdatedAt)
            .FirstOrDefaultAsync(ct);
    }
    public async Task<WorkOrder?> GetByIdWithLinesAsync(Guid id, CancellationToken ct)
    {
        return await _db.WorkOrders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.WorkOrders
            .Include(wo=>wo.Lines) // important : charge les lignes via backing field
            .FirstOrDefaultAsync(x => x.Id == id, ct);

}
