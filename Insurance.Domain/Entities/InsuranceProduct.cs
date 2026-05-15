namespace Insurance.Domain.Entities
{
    public class InsuranceProduct : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // e.g., Auto, Home, Life, Health
        public decimal BasePrice { get; set; }
        public string CoverageDetails { get; set; }
        public int CoverageLimitInDays { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
        public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
    }
}
