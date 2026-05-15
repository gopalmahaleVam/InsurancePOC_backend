using Insurance.Application.Common.Models;
using Insurance.Application.Features.Policies.DTOs;
using MediatR;

namespace Insurance.Application.Features.Policies.Commands;

public class CreatePolicyCommand : IRequest<Result<PolicyResponseDto>>
{
    public required string PolicyNumber { get; set; }
    public required int CustomerId { get; set; }
    public required int InsuranceProductId { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public decimal PremiumAmount { get; set; }
    public string? PaymentFrequency { get; set; }
}

public class UpdatePolicyCommand : IRequest<Result<PolicyResponseDto>>
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

public class DeletePolicyCommand : IRequest<Result<bool>>
{
    public int Id { get; }
    public DeletePolicyCommand(int id) => Id = id;
}
