using Insurance.Application.Common.Models;
using Insurance.Application.Features.InsuranceProducts.Commands;
using Insurance.Application.Features.InsuranceProducts.DTOs;
using Insurance.Application.Features.InsuranceProducts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InsuranceProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InsuranceProductsController> _logger;

    public InsuranceProductsController(IMediator mediator, ILogger<InsuranceProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateInsuranceProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess) return BadRequest(new { success = false, message = result.ErrorMessage });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInsuranceProductByIdQuery(id), cancellationToken);
        if (!result.IsSuccess) return NotFound(new { success = false, message = result.ErrorMessage });
        return Ok(new { success = true, data = result.Data });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetAllInsuranceProductsQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        if (!result.IsSuccess) return BadRequest(new { success = false, message = result.ErrorMessage });
        return Ok(new { success = true, data = result.Data?.Items, pagination = new { pageNumber = result.Data?.PageNumber, pageSize = result.Data?.PageSize, totalItems = result.Data?.TotalItems } });
    }

    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInsuranceProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route id does not match command id");
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess) return NotFound(new { success = false, message = result.ErrorMessage });
        return Ok(new { success = true, data = result.Data });
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteInsuranceProductCommand(id), cancellationToken);
        if (!result.IsSuccess) return NotFound(new { success = false, message = result.ErrorMessage });
        return NoContent();
    }
}
