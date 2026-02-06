using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Domain.WorkOrders;
using Microsoft.EntityFrameworkCore;

namespace InterventionService.Infrastructure.Persistence.Repositories;

public sealed class WorkOrderRepository : IWorkOrderRepository
{
    private readonly InterventionDbContext _db;

    public WorkOrderRepository(InterventionDbContext db) => _db = db;

    public async Task AddAsync(WorkOrder entity, CancellationToken ct)
        => await _db.WorkOrders.AddAsync(entity, ct);

    public async Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.WorkOrders
            .Include("_lines") // important : charge les lignes via backing field
            .FirstOrDefaultAsync(x => x.Id == id, ct);
}
