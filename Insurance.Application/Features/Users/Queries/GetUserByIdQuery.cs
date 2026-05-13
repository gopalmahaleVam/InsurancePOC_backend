using MediatR;
using Insurance.Application.Features.Users.DTOs;

namespace Insurance.Application.Features.Users.Queries
{
    public class GetUserByIdQuery : IRequest<GetUserDto>
    {
        public int Id { get; set; }
    }
}
