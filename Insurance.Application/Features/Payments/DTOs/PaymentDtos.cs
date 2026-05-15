namespace Insurance.Application.Features.Payments.DTOs;

public class CreatePaymentDto
{
    public required string TransactionId { get; set; }
    public required int PolicyId { get; set; }
    public required int CustomerId { get; set; }
    public required decimal Amount { get; set; }
    public required DateTime PaymentDate { get; set; }
    public required string PaymentMethod { get; set; }
    public string? Description { get; set; }
}

public class UpdatePaymentDto
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

public class PaymentResponseDto
{
    public int Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public int PolicyId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
