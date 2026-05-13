using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Users.DTOs;
using Insurance.Application.Features.Users.Queries;
using MediatR;

namespace Insurance.Application.Features.Users.Handlers.QueryHandlers;

/// <summary>
/// Handler for GetUserByIdQuery.
/// Retrieves a single user by ID for viewing user details.
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserResponseDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return Result<UserResponseDto>.Failure($"User with ID {request.Id} not found");

        var userDto = _mapper.Map<UserResponseDto>(user);
        return Result<UserResponseDto>.Success(userDto);
    }
}

/// <summary>
/// Handler for GetAllUsersQuery.
/// Retrieves paginated list of users with optional filtering and sorting.
/// </summary>
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PaginatedResult<UserListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<UserListDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (items, totalCount) = await _unitOfWork.Users.GetPaginatedAsync(
                request.PageNumber, 
                request.PageSize, 
                cancellationToken);

            // Convert to List to allow LINQ filtering in-memory
            var usersList = items.ToList();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                usersList = usersList
                    .Where(u => u.Username.ToLower().Contains(searchLower) ||
                               u.Email.ToLower().Contains(searchLower) ||
                               u.FirstName.ToLower().Contains(searchLower) ||
                               u.LastName.ToLower().Contains(searchLower))
                    .ToList();
            }

            // Apply role filter if provided
            if (request.RoleFilter.HasValue)
            {
                usersList = usersList
                    .Where(u => u.Role == request.RoleFilter.Value)
                    .ToList();
            }

            // Apply active status filter if provided
            if (request.IsActiveFilter.HasValue)
            {
                usersList = usersList
                    .Where(u => u.IsActive == request.IsActiveFilter.Value)
                    .ToList();
            }

            // Apply sorting
            usersList = ApplySorting(usersList, request.SortBy);

            var userDtos = _mapper.Map<List<UserListDto>>(usersList);
            var paginatedResult = new PaginatedResult<UserListDto>(
                userDtos,
                request.PageNumber,
                request.PageSize,
                totalCount
            );

            return Result<PaginatedResult<UserListDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<UserListDto>>.Failure($"Failed to retrieve users: {ex.Message}");
        }
    }

    private static List<Domain.Entities.User> ApplySorting(List<Domain.Entities.User> users, string sortBy)
    {
        if (string.IsNullOrEmpty(sortBy))
            return users.OrderByDescending(u => u.CreatedAt).ToList();

        var descending = sortBy.StartsWith('-');
        var field = descending ? sortBy.Substring(1) : sortBy;

        return field.ToLower() switch
        {
            "username" => descending
                ? users.OrderByDescending(u => u.Username).ToList()
                : users.OrderBy(u => u.Username).ToList(),
            "email" => descending
                ? users.OrderByDescending(u => u.Email).ToList()
                : users.OrderBy(u => u.Email).ToList(),
            "firstname" => descending
                ? users.OrderByDescending(u => u.FirstName).ToList()
                : users.OrderBy(u => u.FirstName).ToList(),
            "lastname" => descending
                ? users.OrderByDescending(u => u.LastName).ToList()
                : users.OrderBy(u => u.LastName).ToList(),
            "createdat" => descending
                ? users.OrderByDescending(u => u.CreatedAt).ToList()
                : users.OrderBy(u => u.CreatedAt).ToList(),
            _ => users.OrderByDescending(u => u.CreatedAt).ToList()
        };
    }
}

/// <summary>
/// Handler for SearchUsersByNameQuery.
/// Searches users by first or last name.
/// </summary>
public class SearchUsersByNameQueryHandler : IRequestHandler<SearchUsersByNameQuery, Result<IEnumerable<UserResponseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SearchUsersByNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<UserResponseDto>>> Handle(SearchUsersByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _unitOfWork.Users.SearchByNameAsync(request.SearchTerm, cancellationToken);
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);
            return Result<IEnumerable<UserResponseDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<UserResponseDto>>.Failure($"Search failed: {ex.Message}");
        }
    }
}

/// <summary>
/// Handler for GetUsersByRoleQuery.
/// Retrieves all users with a specific role.
/// </summary>
public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, Result<IEnumerable<UserResponseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUsersByRoleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<UserResponseDto>>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _unitOfWork.Users.GetByRoleAsync(request.Role, cancellationToken);
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);
            return Result<IEnumerable<UserResponseDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<UserResponseDto>>.Failure($"Failed to retrieve users: {ex.Message}");
        }
    }
}

/// <summary>
/// Handler for CheckUsernameAvailabilityQuery.
/// Checks if a username is available for registration.
/// Returns true if available, false if already taken.
/// </summary>
public class CheckUsernameAvailabilityQueryHandler : IRequestHandler<CheckUsernameAvailabilityQuery, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckUsernameAvailabilityQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CheckUsernameAvailabilityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _unitOfWork.Users.UsernameExistsAsync(request.Username, cancellationToken);
            return Result<bool>.Success(!exists);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to check username availability: {ex.Message}");
        }
    }
}

/// <summary>
/// Handler for CheckEmailAvailabilityQuery.
/// Checks if an email is available for registration.
/// Returns true if available, false if already registered.
/// </summary>
public class CheckEmailAvailabilityQueryHandler : IRequestHandler<CheckEmailAvailabilityQuery, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckEmailAvailabilityQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CheckEmailAvailabilityQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken);
            return Result<bool>.Success(!exists);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to check email availability: {ex.Message}");
        }
    }
}
