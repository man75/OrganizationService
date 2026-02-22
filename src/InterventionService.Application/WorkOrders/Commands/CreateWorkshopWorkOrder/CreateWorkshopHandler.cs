
using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Application.WorkOrders;
using InterventionService.Domain.Enums;
using InterventionService.Domain.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace InterventionService.Application.WorkOrders.Commands.CreateWorkshopWorkOrder;

public sealed class CreateWorkshopHandler
    : IRequestHandler<CreateWorkshopWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IUserContext _current;
    private readonly IWorkDefinitionRepository _defs;
    private readonly IWorkOrderRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateWorkshopHandler(
        IUserContext current,
        IWorkDefinitionRepository defs,
        IWorkOrderRepository repo,
        IUnitOfWork uow)
    {
        _current = current;
        _defs = defs;
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<WorkOrderDto>> Handle(CreateWorkshopWorkOrderCommand request, CancellationToken ct)
    {
        var orgId = _current.OrganizationId;

        // ✅ DefinitionId optionnel : on vérifie seulement si fourni
        if (request.DefinitionId != null & request.DefinitionId != Guid.Empty)
        {
            var definitionOk = await _defs.ExistsActiveAsync(orgId, request.DefinitionId, ct);
            if (!definitionOk)
                return Result<WorkOrderDto>.Failure("Intervention definition not found or not active.");
        }
        var existing = await _repo.GetByWithLignesIdAsync(orgId, request.VehicleId, ct);


        if (existing is not null)
            return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(existing));

        var entity = new WorkOrder(
            id: Guid.NewGuid(),
            organizationId: orgId,
            kind: WorkOrderKind.Workshop,
            scheduledAt: request.ScheduledAt,
            currency: "EUR",
            vehicleId: request.VehicleId,
            clientId: null,
            definitionId: request.DefinitionId, // ✅ peut être null
            technicianId: request.TechnicianId,
            notes: request.Notes
        );

        await _repo.AddAsync(entity, ct);
        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {

            throw;
        }


        return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(entity));
    }
}