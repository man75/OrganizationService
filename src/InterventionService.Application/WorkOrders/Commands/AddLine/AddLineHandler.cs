using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Abstractions;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.WorkOrders;
using MediatR;

namespace InterventionService.Application.WorkOrders.Commands.AddLine;

public sealed class AddLineHandler : IRequestHandler<AddLineCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _repo;
    private readonly IUnitOfWork _uow;

    public AddLineHandler(IWorkOrderRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkOrderDto>> Handle(AddLineCommand request, CancellationToken ct)
    {
        var wo = await _repo.GetByIdAsync(request.WorkOrderId, ct);
        if (wo is null) return Result<WorkOrderDto>.Failure("WorkOrder not found.");

        wo.AddLine(
            request.Type,
            request.Label,
            request.Quantity,
            request.UnitPriceExclTax,
            request.VatRate,
            request.ProductId,
            request.SortOrder
        );

        await _uow.SaveChangesAsync(ct);
        return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(wo));
    }
}
