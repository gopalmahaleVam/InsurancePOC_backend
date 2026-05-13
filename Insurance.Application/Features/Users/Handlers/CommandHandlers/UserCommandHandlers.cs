using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Users.Commands;
using Insurance.Application.Features.Users.DTOs;
using Insurance.Domain.Entities;
using MediatR;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace Insurance.Application.Features.Users.Handlers.CommandHandlers;

/// <summary>
/// Handler for CreateUserCommand.
/// Creates a new user with bcrypt-hashed password and validates username/email uniqueness.
/// Triggered via MediatR pipeline with automatic validation.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserResponseDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if username already exists
        var usernameExists = await _unitOfWork.Users.UsernameExistsAsync(request.Username, cancellationToken);
        if (usernameExists)
            return Result<UserResponseDto>.Failure($"Username '{request.Username}' is already taken");

        // Check if email already exists
        var emailExists = await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken);
        if (emailExists)
            return Result<UserResponseDto>.Failure($"Email '{request.Email}' is already registered");

        // Hash the password using bcrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create new user entity
        var user = new User(
            request.Username,
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            request.Role,
            request.PhoneNumber
        );

        try
        {
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userDto = _mapper.Map<UserResponseDto>(user);
            return Result<UserResponseDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserResponseDto>.Failure($"Failed to create user: {ex.Message}");
        }
    }
}

/// <summary>
/// Handler for UpdateUserCommand.
/// Updates user profile information (but not password).
/// Validates email uniqueness if email is being changed.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserResponseDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return Result<UserResponseDto>.Failure($"User with ID {request.Id} not found");

        // If email is being changed, check if it's already taken
        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            var emailExists = await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken);
            if (emailExists)
                return Result<UserResponseDto>.Failure($"Email '{request.Email}' is already registered");

            user.Email = request.Email;
        }

        // Update optional fields only if provided
        if (!string.IsNullOrEmpty(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.LastName))
            user.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;

        if (request.Role.HasValue)
            user.Role = request.Role.Value;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userDto = _mapper.Map<UserResponseDto>(user);
            return Result<UserResponseDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserResponseDto>.Failure($"Failed to update user: {ex.Message}");
        }
    }
}

/// <summary>
/// Handler for DeleteUserCommand.
/// Soft-deletes a user (marks IsDeleted = true, does not remove from database).
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _unitOfWork.Users.DeleteAsync(request.Id, cancellationToken);
            if (!deleted)
                return Result<bool>.Failure($"User with ID {request.Id} not found");

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete user: {ex.Message}");
        }
    }
}
