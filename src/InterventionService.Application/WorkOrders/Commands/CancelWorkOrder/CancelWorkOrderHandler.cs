using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.WorkOrders;
using MediatR;

namespace InterventionService.Application.WorkOrders.Commands.CancelWorkOrder;

public sealed class CancelWorkOrderHandler : IRequestHandler<CancelWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _repo;
    private readonly IUnitOfWork _uow;

    public CancelWorkOrderHandler(IWorkOrderRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkOrderDto>> Handle(CancelWorkOrderCommand request, CancellationToken ct)
    {
        var wo = await _repo.GetByIdAsync(request.WorkOrderId, ct);
        if (wo is null) return Result<WorkOrderDto>.Failure("WorkOrder not found.");

        wo.Cancel(request.Reason);
        await _uow.SaveChangesAsync(ct);

        return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(wo));
    }
}
