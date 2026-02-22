using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Domain.WorkOrders;

namespace InterventionService.Application.Abstractions.Repositories;

public interface IWorkOrderRepository
{
    Task AddAsync(WorkOrder entity, CancellationToken ct);
    Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<WorkOrder?> GetByIdWithLinesAsync(Guid id, CancellationToken ct);
    Task<WorkOrder> GetByWithLignesIdAsync(Guid orgaId, Guid vehicleId, CancellationToken ct);
}
