using Insurance.Application.Features.Claims.Commands;
using Insurance.Application.Features.Claims.DTOs;
using Insurance.Application.Features.Claims.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClaimsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateClaimDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateClaimCommand
        {
            ClaimNumber = dto.ClaimNumber,
            PolicyId = dto.PolicyId,
            CustomerId = dto.CustomerId,
            ClaimDate = dto.ClaimDate,
            Description = dto.Description,
            ClaimAmount = dto.ClaimAmount
        };
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess) return BadRequest(new { success = false, message = result.ErrorMessage });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClaimByIdQuery(id), cancellationToken);
        if (!result.IsSuccess) return NotFound(new { success = false, message = result.ErrorMessage });
        return Ok(new { success = true, data = result.Data });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllClaimsQuery(pageNumber, pageSize), cancellationToken);
        if (!result.IsSuccess) return BadRequest(new { success = false, message = result.ErrorMessage });
        return Ok(new { success = true, data = result.Data?.Items, pagination = new { pageNumber = result.Data?.PageNumber, pageSize = result.Data?.PageSize, totalItems = result.Data?.TotalItems } });
    }

    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateClaimDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id) return BadRequest("Route id does not match payload id");
        var command = new UpdateClaimCommand
        {
            Id = dto.Id,
            ClaimNumber = dto.ClaimNumber,
            PolicyId = dto.PolicyId,
            CustomerId = dto.CustomerId,
            ClaimDate = dto.ClaimDate,
            Description = dto.Description,
            ClaimAmount = dto.ClaimAmount,
            Status = dto.Status,
            ResolutionDate = dto.ResolutionDate,
            ResolutionNotes = dto.ResolutionNotes
        };
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess) return NotFound(new { success = false, message = result.ErrorMessage });
        return Ok(new { success = true, data = result.Data });
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteClaimCommand(id), cancellationToken);
        if (!result.IsSuccess) return NotFound(new { success = false, message = result.ErrorMessage });
        return NoContent();
    }
}
