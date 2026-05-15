using Insurance.Domain.Entities;

namespace Insurance.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        // Navigation properties
        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
        public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
