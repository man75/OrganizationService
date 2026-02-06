using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.WorkOrders;
using MediatR;

namespace InterventionService.Application.WorkOrders.Queries.GetWorkOrderById;

public sealed class GetWorkOrderByIdHandler : IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _repo;

    public GetWorkOrderByIdHandler(IWorkOrderRepository repo) => _repo = repo;

    public async Task<Result<WorkOrderDto>> Handle(GetWorkOrderByIdQuery request, CancellationToken ct)
    {
        var wo = await _repo.GetByIdAsync(request.Id, ct);
        if (wo is null) return Result<WorkOrderDto>.Failure("WorkOrder not found.");
        return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(wo));
    }
}
