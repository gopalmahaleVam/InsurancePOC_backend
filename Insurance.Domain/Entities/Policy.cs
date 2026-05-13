using Insurance.Domain.Enums;

namespace Insurance.Domain.Entities
{
    public class Policy
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; }
        public int CustomerId { get; set; }
        public int InsuranceProductId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PremiumAmount { get; set; }
        public PolicyStatus Status { get; set; } = PolicyStatus.Active;
        public string PaymentFrequency { get; set; } = "Annual"; // Annual, Monthly, etc.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Customer Customer { get; set; }
        public InsuranceProduct InsuranceProduct { get; set; }
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
