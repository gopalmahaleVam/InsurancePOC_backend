namespace Insurance.Domain.Entities
{
    public class Quote
    {
        public int Id { get; set; }
        public string QuoteNumber { get; set; }
        public int CustomerId { get; set; }
        public int InsuranceProductId { get; set; }
        public decimal QuotedAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected, Expired, etc.
        public DateTime ExpiryDate { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Customer Customer { get; set; }
        public InsuranceProduct InsuranceProduct { get; set; }
    }
}
