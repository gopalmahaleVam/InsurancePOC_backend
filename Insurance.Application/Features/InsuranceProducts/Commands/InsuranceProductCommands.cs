using Insurance.Application.Common.Models;
using Insurance.Application.Features.InsuranceProducts.DTOs;
using MediatR;

namespace Insurance.Application.Features.InsuranceProducts.Commands;

public class CreateInsuranceProductCommand : IRequest<Result<InsuranceProductResponseDto>>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Type { get; set; }
    public decimal BasePrice { get; set; }
    public string? CoverageDetails { get; set; }
    public int CoverageLimitInDays { get; set; }
}

public class UpdateInsuranceProductCommand : IRequest<Result<InsuranceProductResponseDto>>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public decimal? BasePrice { get; set; }
    public string? CoverageDetails { get; set; }
    public int? CoverageLimitInDays { get; set; }
    public bool? IsActive { get; set; }
}

public class DeleteInsuranceProductCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
    public DeleteInsuranceProductCommand(int id) => Id = id;
}
