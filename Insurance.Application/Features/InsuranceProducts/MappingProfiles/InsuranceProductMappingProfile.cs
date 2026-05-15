using AutoMapper;
using Insurance.Application.Features.InsuranceProducts.DTOs;
using Insurance.Domain.Entities;

namespace Insurance.Application.Features.InsuranceProducts.MappingProfiles;

public class InsuranceProductMappingProfile : Profile
{
    public InsuranceProductMappingProfile()
    {
        CreateMap<InsuranceProduct, InsuranceProductResponseDto>();
        CreateMap<CreateInsuranceProductDto, InsuranceProduct>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember is not null));
        CreateMap<UpdateInsuranceProductDto, InsuranceProduct>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember is not null));
    }
}
