using System.ComponentModel.DataAnnotations;
using Insurance.Domain.Enums;

namespace Insurance.Domain.Entities
{
    public class Claim
    {
        public int Id { get; set; }
        public string ClaimNumber { get; set; }
        public int PolicyId { get; set; }
        public int CustomerId { get; set; }
        public DateTime ClaimDate { get; set; }
        public string Description { get; set; }
        public decimal ClaimAmount { get; set; }
        public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
        public DateTime? ResolutionDate { get; set; }
        public string? ResolutionNotes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Policy Policy { get; set; }
        public Customer Customer { get; set; }
    }
}
