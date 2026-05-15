namespace Insurance.Application.Features.InsuranceProducts.DTOs;

public class CreateInsuranceProductDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Type { get; set; }
    public decimal BasePrice { get; set; }
    public string? CoverageDetails { get; set; }
    public int CoverageLimitInDays { get; set; }
}

public class UpdateInsuranceProductDto
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

public class InsuranceProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string? CoverageDetails { get; set; }
    public int CoverageLimitInDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
