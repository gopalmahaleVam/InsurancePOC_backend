namespace Insurance.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string TransactionId { get; set; }
        public int PolicyId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } // Credit Card, Bank Transfer, Check, etc.
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded, etc.
        public string Description { get; set; }

        // Navigation properties
        public Policy Policy { get; set; }
        public Customer Customer { get; set; }
    }
}
