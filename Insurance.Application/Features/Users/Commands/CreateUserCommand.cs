
using Insurance.Domain.Enums;
using MediatR;

namespace Insurance.Application.Features.Users.Commands
{
    
public class CreateUserCommand : IRequest<int>
{
    public string Username { get; }
    public string Email { get; }
    public string PasswordHash { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string PhoneNumber { get; }
    public UserRole Role { get; }

    public CreateUserCommand(
        string username,
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        string phoneNumber,
        UserRole role)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Role = role;
    }
}

}