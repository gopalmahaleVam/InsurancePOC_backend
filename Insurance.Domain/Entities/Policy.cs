using Insurance.Domain.Enums;

namespace Insurance.Domain.Entities
{
    public class Policy : BaseEntity
    {
        public string PolicyNumber { get; set; }
        public int CustomerId { get; set; }
        public int InsuranceProductId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PremiumAmount { get; set; }
        public PolicyStatus Status { get; set; } = PolicyStatus.Active;
        public string PaymentFrequency { get; set; } = "Annual"; // Annual, Monthly, etc.

        // Navigation properties
        public Customer Customer { get; set; }
        public InsuranceProduct InsuranceProduct { get; set; }
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
