using MediatR;
using Insurance.Domain.Entities;
using Insurance.Domain.Interfaces;
using Insurance.Application.Features.Users.Commands;
using AutoMapper;

namespace Insurance.Application.Features.Users.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }


public async Task<int> Handle(CreateUserCommand request, CancellationToken ct)
{
    var user = _mapper.Map<User>(request);

    var createdUser = await _userRepository.AddUserAsync(user);

    return createdUser.Id;
}

        // public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        // {
        //     // ✅ Mapping (Command → Domain)
        //     var user = new User
        //     {
        //         Username = request.Username,
        //         Email = request.Email,
        //         PasswordHash = request.PasswordHash,
        //         FirstName = request.FirstName,
        //         LastName = request.LastName,
        //         PhoneNumber = request.PhoneNumber,
        //         Role = request.Role,
        //         CreatedAt = DateTime.UtcNow,
        //         UpdatedAt = DateTime.UtcNow
        //     };

        //     var createdUser = await _userRepository.AddUserAsync(user);

        //     return createdUser.Id;
        // }
    }

}