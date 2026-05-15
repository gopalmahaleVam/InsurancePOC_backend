namespace Insurance.Application.Features.Claims.DTOs;

public class CreateClaimDto
{
    public required string ClaimNumber { get; set; }
    public required int PolicyId { get; set; }
    public required int CustomerId { get; set; }
    public required DateTime ClaimDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal ClaimAmount { get; set; }
}

public class UpdateClaimDto
{
    public int Id { get; set; }
    public string? ClaimNumber { get; set; }
    public int? PolicyId { get; set; }
    public int? CustomerId { get; set; }
    public DateTime? ClaimDate { get; set; }
    public string? Description { get; set; }
    public decimal? ClaimAmount { get; set; }
    public Insurance.Domain.Enums.ClaimStatus? Status { get; set; }
    public DateTime? ResolutionDate { get; set; }
    public string? ResolutionNotes { get; set; }
}

public class ClaimResponseDto
{
    public int Id { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    public int PolicyId { get; set; }
    public int CustomerId { get; set; }
    public DateTime ClaimDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal ClaimAmount { get; set; }
    public Insurance.Domain.Enums.ClaimStatus Status { get; set; }
    public DateTime? ResolutionDate { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
