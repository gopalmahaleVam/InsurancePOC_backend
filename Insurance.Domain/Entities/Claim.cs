using System.ComponentModel.DataAnnotations;
using Insurance.Domain.Enums;

namespace Insurance.Domain.Entities
{
    public class Claim : BaseEntity
    {
        public string ClaimNumber { get; set; }
        public int PolicyId { get; set; }
        public int CustomerId { get; set; }
        public DateTime ClaimDate { get; set; }
        public string Description { get; set; }
        public decimal ClaimAmount { get; set; }
        public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
        public DateTime? ResolutionDate { get; set; }
        public string? ResolutionNotes { get; set; }

        // Navigation properties
        public Policy Policy { get; set; }
        public Customer Customer { get; set; }
    }
}
