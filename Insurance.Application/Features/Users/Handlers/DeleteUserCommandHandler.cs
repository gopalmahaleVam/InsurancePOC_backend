using MediatR;
using Insurance.Application.Features.Users.Commands;
using Insurance.Domain.Interfaces;

namespace Insurance.Application.Features.Users.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.Id);
            
            if (user == null)
                throw new KeyNotFoundException($"User with Id {request.Id} not found");

            return await _userRepository.DeleteUserAsync(request.Id);
        }
    }
}
