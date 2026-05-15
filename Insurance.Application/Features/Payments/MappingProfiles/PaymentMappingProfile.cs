using AutoMapper;
using Insurance.Application.Features.Payments.DTOs;
using Insurance.Domain.Entities;

namespace Insurance.Application.Features.Payments.MappingProfiles;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, PaymentResponseDto>();
        CreateMap<CreatePaymentDto, Payment>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember is not null));
        CreateMap<UpdatePaymentDto, Payment>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember is not null));
    }
}
