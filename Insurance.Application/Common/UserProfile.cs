using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Insurance.Application.Features.Users.DTOs;
using Insurance.Domain.Entities;
using Insurance.Application.Features.Users.Commands;

namespace Insurance.Application.Common
{
    public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserCommand, User>();
        //CreateMap<User, UserResponseDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
    }
}
}