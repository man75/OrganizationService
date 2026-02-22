using MediatR;
using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;

using InterventionService.Domain.Enums;

namespace InterventionService.Application.WorkOrders.Commands.AddProductLine;

public sealed class AddWorkOrderProductLineCommandHandler
    : IRequestHandler<AddWorkOrderProductLineCommand, Result<WorkOrderDto>>
{
    private readonly IUserContext _current;
    private readonly IWorkOrderRepository _orderRepo;
    private readonly IStockGateway _stock;
    private readonly IUnitOfWork _uow;

    public AddWorkOrderProductLineCommandHandler(
        IUserContext current,
        IWorkOrderRepository orderRepo,
        IStockGateway stock,
        IUnitOfWork uow)
    {
        _current = current;
        _orderRepo = orderRepo;
        _stock = stock;
        _uow = uow;
    }

    public async Task<Result<WorkOrderDto>> Handle(AddWorkOrderProductLineCommand request, CancellationToken ct)
    {
        var orgId = _current.OrganizationId;

        var order = await _orderRepo.GetByIdAsync(request.WorkOrderId, ct);
        if (order is null)
            return Result<WorkOrderDto>.Failure("WorkOrder not found.");

        var pricing = await _stock.GetProductPricingAsync(orgId, request.ProductId, ct);
        if (pricing is null)
            return Result<WorkOrderDto>.Failure("Product not found in stock.");

        // ✅ anti-doublon: si déjà présent => augmente qty
        var existing = order.Lines.FirstOrDefault(l => l.ProductId == request.ProductId);
        if (existing is not null)
        {
            existing.IncreaseQuantity(request.Quantity); // méthode domaine
        }
        else
        {
            order.AddLine(
                type: WorkOrderLineType.Product,
                label: pricing.Designation,
                quantity: request.Quantity,
                unitPriceExclTax: pricing.UnitPriceExclTax,
                vatRate: pricing.VatRate,
                productId: request.ProductId,
                sortOrder: order.Lines.Count
            );
        }

        await _uow.SaveChangesAsync(ct);

        return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(order));
    }
}