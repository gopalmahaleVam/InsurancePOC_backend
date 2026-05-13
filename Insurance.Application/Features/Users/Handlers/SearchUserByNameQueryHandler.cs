using MediatR;
using Insurance.Application.Features.Users.DTOs;
using Insurance.Application.Features.Users.Queries;
using Insurance.Domain.Interfaces;

namespace Insurance.Application.Features.Users.Handlers
{
    public class SearchUserByNameQueryHandler : IRequestHandler<SearchUserByNameQuery, List<GetUserDto>>
    {
        private readonly IUserRepository _userRepository;

        public SearchUserByNameQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<GetUserDto>> Handle(SearchUserByNameQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.SearchUsersByNameAsync(request.FirstName, request.LastName);

            return users.Select(u => new GetUserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToList();
        }
    }
}
