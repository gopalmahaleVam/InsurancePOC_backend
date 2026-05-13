using MediatR;
using Insurance.Application.Features.Users.DTOs;

namespace Insurance.Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<GetUserDto>
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public Insurance.Domain.Enums.UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }
}
