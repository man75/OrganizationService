using MediatR;
using Microsoft.AspNetCore.Mvc;
using InterventionService.API.Contracts.WorkDefinitions;
using InterventionService.Application.WorkDefinitions.Commands.CreateWorkDefinition;
using InterventionService.Application.WorkDefinitions.Queries.GetActiveWorkDefinitions;

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
            Name: request.Name,
            Type: request.Type
        );

        var res = await _mediator.Send(cmd, ct);
        if (!res.IsSuccess) return BadRequest(new { res.Error });

        return Ok(res.Value);
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
}
