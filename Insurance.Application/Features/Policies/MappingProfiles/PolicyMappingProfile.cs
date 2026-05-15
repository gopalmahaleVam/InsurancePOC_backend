using AutoMapper;
using Insurance.Application.Features.Policies.DTOs;
using Insurance.Domain.Entities;

namespace Insurance.Application.Features.Policies.MappingProfiles;

public class PolicyMappingProfile : Profile
{
    public PolicyMappingProfile()
    {
        CreateMap<Policy, PolicyResponseDto>();
        CreateMap<CreatePolicyDto, Policy>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember is not null));
        CreateMap<UpdatePolicyDto, Policy>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember is not null));
    }
}
