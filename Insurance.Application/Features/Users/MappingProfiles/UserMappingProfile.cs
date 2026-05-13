using AutoMapper;
using Insurance.Application.Features.Users.DTOs;
using Insurance.Domain.Entities;

namespace Insurance.Application.Features.Users.MappingProfiles;

/// <summary>
/// AutoMapper profile for User entity and DTO mappings.
/// Defines bidirectional mappings between domain entities and data transfer objects.
/// Ensures clean separation between domain and API layers.
/// </summary>
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // User Entity to UserResponseDto
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));

        // User Entity to UserListDto  
        CreateMap<User, UserListDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));

        // CreateUserDto to User Entity (for command mapping)
        // Note: Password is NOT mapped here - it's handled separately in the handler where it's bcrypt-hashed
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore());

        // UpdateUserDto to User Entity (for partial updates)
        // This is typically handled by the handler where it selectively updates properties
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Username, opt => opt.Ignore()) // Username cannot be changed
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Password changed via separate endpoint
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
    }
}
