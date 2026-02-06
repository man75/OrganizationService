using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.WorkOrders;
using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkOrders;
using MediatR;

namespace InterventionService.Application.WorkOrders.Commands.CreateCounterSale;

public sealed class CreateCounterSaleHandler
    : IRequestHandler<CreateCounterSaleCommand, Result<WorkOrderDto>>
{
    private readonly IUserContext _current;
    private readonly IWorkOrderRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateCounterSaleHandler(IUserContext current, IWorkOrderRepository repo, IUnitOfWork uow)
    {
        _current = current;
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkOrderDto>> Handle(CreateCounterSaleCommand request, CancellationToken ct)
    {
        var entity = new WorkOrder(
            id: Guid.NewGuid(),
            organizationId: _current.OrganizationId,
            kind: WorkOrderKind.CounterSale,
            scheduledAt: request.ScheduledAt,
            currency: "EUR",
            vehicleId: null,
            clientId: request.ClientId,
            definitionId: null,
            technicianId: null,
            notes: request.Notes
        );

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(entity));
    }
}
