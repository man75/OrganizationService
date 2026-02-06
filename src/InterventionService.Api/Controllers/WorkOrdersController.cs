using Microsoft.AspNetCore.Mvc;
using MediatR;
using InterventionService.Application.WorkOrders.Commands.CreateWorkshopWorkOrder;
using InterventionService.Application.WorkOrders.Commands.CreateCounterSale;
using InterventionService.Application.WorkOrders.Commands.AddLine;
using InterventionService.Application.WorkOrders.Commands.StartWorkOrder;
using InterventionService.Application.WorkOrders.Commands.CompleteWorkOrder;
using InterventionService.Application.WorkOrders.Commands.CancelWorkOrder;
using InterventionService.Application.WorkOrders.Queries.GetWorkOrderById;
using InterventionService.API.Contracts.WorkOrders;

namespace InterventionService.API.Controllers;

[ApiController]
[Route("workorders")]
public sealed class WorkOrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public WorkOrdersController(IMediator mediator) => _mediator = mediator;

    // -----------------------
    // GET by id
    // -----------------------
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var res = await _mediator.Send(new GetWorkOrderByIdQuery(id), ct);
        return res.IsSuccess ? Ok(res.Value) : NotFound(new { res.Error });
    }

    // -----------------------
    // CREATE workshop workorder
    // -----------------------
    [HttpPost("workshop")]
    public async Task<IActionResult> CreateWorkshop([FromBody] CreateWorkshopWorkOrderRequest request, CancellationToken ct)
    {
        var cmd = new CreateWorkshopWorkOrderCommand(
            request.VehicleId,
            request.DefinitionId,
            request.ScheduledAt,
            request.TechnicianId,
            request.Notes
        );

        var res = await _mediator.Send(cmd, ct);
        if (!res.IsSuccess) return BadRequest(new { res.Error });

        return CreatedAtAction(nameof(GetById), new { id = res.Value!.Id }, res.Value);
    }

    // -----------------------
    // CREATE counter sale
    // -----------------------
    [HttpPost("counter-sale")]
    public async Task<IActionResult> CreateCounterSale([FromBody] CreateCounterSaleRequest request, CancellationToken ct)
    {
        var cmd = new CreateCounterSaleCommand(
            request.ClientId,
            request.ScheduledAt,
            request.Notes
        );

        var res = await _mediator.Send(cmd, ct);
        if (!res.IsSuccess) return BadRequest(new { res.Error });

        return CreatedAtAction(nameof(GetById), new { id = res.Value!.Id }, res.Value);
    }

    // -----------------------
    // ADD LINE
    // -----------------------
    [HttpPost("{id:guid}/lines")]
    public async Task<IActionResult> AddLine(Guid id, [FromBody] AddLineRequest request, CancellationToken ct)
    {
        var cmd = new AddLineCommand(
            WorkOrderId: id,
            Type: request.Type,
            Label: request.Label,
            Quantity: request.Quantity,
            UnitPriceExclTax: request.UnitPriceExclTax,
            VatRate: request.VatRate,
            ProductId: request.ProductId,
            SortOrder: request.SortOrder
        );

        var res = await _mediator.Send(cmd, ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(new { res.Error });
    }

    // -----------------------
    // START
    // -----------------------
    [HttpPatch("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct)
    {
        var res = await _mediator.Send(new StartWorkOrderCommand(id), ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(new { res.Error });
    }

    // -----------------------
    // COMPLETE
    // -----------------------
    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        var res = await _mediator.Send(new CompleteWorkOrderCommand(id), ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(new { res.Error });
    }

    // -----------------------
    // CANCEL
    // -----------------------
    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelWorkOrderRequest request, CancellationToken ct)
    {
        var res = await _mediator.Send(new CancelWorkOrderCommand(id, request.Reason), ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(new { res.Error });
    }
}
