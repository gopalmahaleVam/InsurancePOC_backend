namespace Insurance.Application.Features.Customers.DTOs;

/// <summary>
/// DTO for creating a new customer via API.
/// Contains only the fields required for customer creation; excludes audit fields and system-generated properties.
/// </summary>
public class CreateCustomerDto
{
    /// <summary>
    /// Customer's first name (1-100 characters).
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Customer's last name (1-100 characters).
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Valid email address (must be unique in system).
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Phone number for contact purposes.
    /// </summary>
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// Street address for the customer.
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// City where the customer resides.
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// State abbreviation (e.g., "CA", "NY").
    /// </summary>
    public required string State { get; set; }

    /// <summary>
    /// Zip code for the customer's location.
    /// </summary>
    public required string ZipCode { get; set; }

    /// <summary>
    /// Customer's date of birth.
    /// </summary>
    public required DateTime DateOfBirth { get; set; }
}

/// <summary>
/// DTO for updating an existing customer via API.
/// Contains fields that can be modified after customer creation.
/// </summary>
public class UpdateCustomerDto
{
    /// <summary>
    /// Customer's first name (1-100 characters).
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Customer's last name (1-100 characters).
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Email address for contact (must be unique if changed).
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Phone number for contact purposes.
    /// </summary>
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// Street address for the customer.
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// City where the customer resides.
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// State abbreviation (e.g., "CA", "NY").
    /// </summary>
    public required string State { get; set; }

    /// <summary>
    /// Zip code for the customer's location.
    /// </summary>
    public required string ZipCode { get; set; }

    /// <summary>
    /// Customer's date of birth.
    /// </summary>
    public required DateTime DateOfBirth { get; set; }
}

/// <summary>
/// DTO for detailed customer response from API.
/// Contains all customer information including audit timestamps.
/// </summary>
public class CustomerResponseDto
{
    /// <summary>
    /// Unique customer identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Customer's first name.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Customer's last name.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Full name (computed: FirstName LastName).
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Customer's email address.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Customer's phone number.
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Street address.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// City.
    /// </summary>
    public string City { get; set; } = null!;

    /// <summary>
    /// State.
    /// </summary>
    public string State { get; set; } = null!;

    /// <summary>
    /// Zip code.
    /// </summary>
    public string ZipCode { get; set; } = null!;

    /// <summary>
    /// Complete address formatted as: Street, City, State Zip
    /// </summary>
    public string FullAddress => $"{Address}, {City}, {State} {ZipCode}";

    /// <summary>
    /// Customer's date of birth.
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Customer's age calculated from date of birth.
    /// </summary>
    public int Age
    {
        get
        {
            var today = DateTime.UtcNow;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age))
                age--;
            return age;
        }
    }

    /// <summary>
    /// Timestamp when the customer was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the customer was last updated (UTC).
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for customer list items returned from pagination queries.
/// Contains essential information for list displays.
/// </summary>
public class CustomerListDto
{
    /// <summary>
    /// Unique customer identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Customer's first name.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Customer's last name.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Full name (computed: FirstName LastName).
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Customer's email address.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Customer's phone number.
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// City where customer resides.
    /// </summary>
    public string City { get; set; } = null!;

    /// <summary>
    /// State where customer resides.
    /// </summary>
    public string State { get; set; } = null!;

    /// <summary>
    /// Customer's date of birth.
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Customer's age calculated from date of birth.
    /// </summary>
    public int Age
    {
        get
        {
            var today = DateTime.UtcNow;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age))
                age--;
            return age;
        }
    }
}
