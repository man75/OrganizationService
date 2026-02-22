using MediatR;
using Microsoft.AspNetCore.Mvc;
using InterventionService.API.Contracts.WorkDefinitions;
using InterventionService.Application.WorkDefinitions.Commands.CreateWorkDefinition;
using InterventionService.Application.WorkDefinitions.Queries.GetActiveWorkDefinitions;
using InterventionService.Application.WorkDefinitions.Commands.AddWorkDefinitionLine;
using InterventionService.Api.Contracts.Requests;
using InterventionService.Application.WorkDefinitions.Commands.AddWorkDefinitionLines;
using InterventionService.Application.WorkDefinitions.Queries.GetWorkDefinitionById;

namespace InterventionService.API.Controllers;

[ApiController]
[Route("work-definitions")]
public sealed class WorkDefinitionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public WorkDefinitionsController(IMediator mediator) => _mediator = mediator;

    // -----------------------
    // POST: create work definition
    // -----------------------
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkDefinitionRequest request, CancellationToken ct)
    {
        var cmd = new CreateWorkDefinitionCommand(
             request.Name,
            request.Type,
            request.EstimatedMinutes,
            request.Notes
        );

        var res = await _mediator.Send(cmd, ct);
        if (!res.IsSuccess) return BadRequest(new { res.Error });

        return Ok(res.Value);
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var res = await _mediator.Send(new GetWorkDefinitionByIdQuery(id), ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(new { res.Error });
    }

    [HttpPost("{id:guid}/line")]
    public async Task<IActionResult> AddLine(
    Guid id,
    [FromBody] AddWorkDefinitionLineRequest request)
    {
        var result = await _mediator.Send(
            new AddWorkDefinitionLineCommand(
                id,
                request.Type,
                request.Label,
                request.Quantity,
                request.ProductId,
                request.UnitPriceExclTax,
                request.VatRate,
                request.SortOrder
            ));

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
    // -----------------------
    // GET: active work definitions
    // -----------------------
    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken ct)
    {
        var res = await _mediator.Send(new GetActiveWorkDefinitionsQuery(), ct);
        return res.IsSuccess ? Ok(res.Value) : BadRequest(new { res.Error });
    }
    [HttpPost("{id:guid}/lines")]
    public async Task<IActionResult> AddLines(
    Guid id,
    [FromBody] List<AddWorkDefinitionLineRequest> request)
    {
        var result = await _mediator.Send(
            new AddWorkDefinitionLinesCommand(
                id,
                request.Select(x => new AddWorkDefinitionLineItem(
                    x.Type,
                    x.Label,
                    x.Quantity,
                    x.ProductId,
                    x.UnitPriceExclTax,
                    x.VatRate,
                    x.SortOrder
                )).ToList()
            ));

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
