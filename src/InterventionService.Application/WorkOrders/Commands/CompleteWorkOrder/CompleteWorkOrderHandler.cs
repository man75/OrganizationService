using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.WorkOrders;
using MediatR;

namespace InterventionService.Application.WorkOrders.Commands.CompleteWorkOrder;

public sealed class CompleteWorkOrderHandler : IRequestHandler<CompleteWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _repo;
    private readonly IUnitOfWork _uow;

    public CompleteWorkOrderHandler(IWorkOrderRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkOrderDto>> Handle(CompleteWorkOrderCommand request, CancellationToken ct)
    {
        var wo = await _repo.GetByIdAsync(request.WorkOrderId, ct);
        if (wo is null) return Result<WorkOrderDto>.Failure("WorkOrder not found.");

        wo.Complete();
        await _uow.SaveChangesAsync(ct);

        return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(wo));
    }
}
