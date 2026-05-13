namespace Insurance.Domain.Entities
{
    public class InsuranceProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // e.g., Auto, Home, Life, Health
        public decimal BasePrice { get; set; }
        public string CoverageDetails { get; set; }
        public int CoverageLimitInDays { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
        public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
    }
}
