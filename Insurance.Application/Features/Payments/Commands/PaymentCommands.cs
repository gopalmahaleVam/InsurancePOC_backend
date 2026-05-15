using Insurance.Application.Common.Models;
using Insurance.Application.Features.Payments.DTOs;
using MediatR;

namespace Insurance.Application.Features.Payments.Commands;

public class CreatePaymentCommand : IRequest<Result<PaymentResponseDto>>
{
    public required string TransactionId { get; set; }
    public required int PolicyId { get; set; }
    public required int CustomerId { get; set; }
    public required decimal Amount { get; set; }
    public required DateTime PaymentDate { get; set; }
    public required string PaymentMethod { get; set; }
    public string? Description { get; set; }
}

public class UpdatePaymentCommand : IRequest<Result<PaymentResponseDto>>
{
    public int Id { get; set; }
    public string? TransactionId { get; set; }
    public int? PolicyId { get; set; }
    public int? CustomerId { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
}

public class DeletePaymentCommand : IRequest<Result<bool>>
{
    public int Id { get; }
    public DeletePaymentCommand(int id) => Id = id;
}
