namespace Insurance.Application.Features.Policies.DTOs;

public class CreatePolicyDto
{
    public required string PolicyNumber { get; set; }
    public required int CustomerId { get; set; }
    public required int InsuranceProductId { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public decimal PremiumAmount { get; set; }
    public string? PaymentFrequency { get; set; }
}

public class UpdatePolicyDto
{
    public int Id { get; set; }
    public string? PolicyNumber { get; set; }
    public int? CustomerId { get; set; }
    public int? InsuranceProductId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? PremiumAmount { get; set; }
    public string? PaymentFrequency { get; set; }
    public Insurance.Domain.Enums.PolicyStatus? Status { get; set; }
}

public class PolicyResponseDto
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int InsuranceProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PremiumAmount { get; set; }
    public string PaymentFrequency { get; set; } = string.Empty;
    public Insurance.Domain.Enums.PolicyStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
