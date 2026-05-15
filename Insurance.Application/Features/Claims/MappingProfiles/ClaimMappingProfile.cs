using AutoMapper;
using Insurance.Application.Features.Claims.DTOs;
using Insurance.Domain.Entities;

namespace Insurance.Application.Features.Claims.MappingProfiles;

public class ClaimMappingProfile : Profile
{
    public ClaimMappingProfile()
    {
        CreateMap<Claim, ClaimResponseDto>();
        CreateMap<CreateClaimDto, Claim>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember is not null));
        CreateMap<UpdateClaimDto, Claim>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember is not null));
    }
}
