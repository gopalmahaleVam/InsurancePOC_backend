using Insurance.Application.Features.Payments.Commands;
using Insurance.Application.Features.Payments.DTOs;
using Insurance.Application.Features.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreatePaymentDto dto, CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand
        {
            TransactionId = dto.TransactionId,
            PolicyId = dto.PolicyId,
            CustomerId = dto.CustomerId,
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate,
            PaymentMethod = dto.PaymentMethod,
            Description = dto.Description
        };

        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess) return BadRequest(new { success = false, message = result.ErrorMessage });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPaymentByIdQuery(id), cancellationToken);
        if (!result.IsSuccess) return NotFound(new { success = false, message = result.ErrorMessage });
        return Ok(new { success = true, data = result.Data });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllPaymentsQuery(pageNumber, pageSize), cancellationToken);
        if (!result.IsSuccess) return BadRequest(new { success = false, message = result.ErrorMessage });
        return Ok(new { success = true, data = result.Data?.Items, pagination = new { pageNumber = result.Data?.PageNumber, pageSize = result.Data?.PageSize, totalItems = result.Data?.TotalItems } });
    }

    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePaymentDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id) return BadRequest("Route id does not match payload id");
        var command = new UpdatePaymentCommand
        {
            Id = dto.Id,
            TransactionId = dto.TransactionId,
            PolicyId = dto.PolicyId,
            CustomerId = dto.CustomerId,
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate,
            PaymentMethod = dto.PaymentMethod,
            Status = dto.Status,
            Description = dto.Description
        };

        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess) return NotFound(new { success = false, message = result.ErrorMessage });
        return Ok(new { success = true, data = result.Data });
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePaymentCommand(id), cancellationToken);
        if (!result.IsSuccess) return NotFound(new { success = false, message = result.ErrorMessage });
        return NoContent();
    }
}
