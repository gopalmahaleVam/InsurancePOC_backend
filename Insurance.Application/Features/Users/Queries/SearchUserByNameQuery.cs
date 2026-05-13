using MediatR;
using Insurance.Application.Features.Users.DTOs;

namespace Insurance.Application.Features.Users.Queries
{
    public class SearchUserByNameQuery : IRequest<List<GetUserDto>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
