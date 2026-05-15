using AutoMapper;
using Insurance.Application.Features.Customers.Commands;
using Insurance.Application.Features.Customers.DTOs;
using Insurance.Domain.Entities;

namespace Insurance.Application.Features.Customers.MappingProfiles;

/// <summary>
/// AutoMapper profile for Customer entity mappings.
/// Defines how Customer entities map to/from DTOs.
/// </summary>
public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        // Customer -> CustomerResponseDto mapping
        CreateMap<Customer, CustomerResponseDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => $"{src.Address}, {src.City}, {src.State} {src.ZipCode}"));

        // Customer -> CustomerListDto mapping
        CreateMap<Customer, CustomerListDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

        // CreateCustomerCommand -> Customer mapping
        CreateMap<CreateCustomerCommand, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        // UpdateCustomerCommand -> Customer mapping
        CreateMap<UpdateCustomerCommand, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // CreateCustomerDto -> CreateCustomerCommand mapping
        CreateMap<CreateCustomerDto, CreateCustomerCommand>();

        // UpdateCustomerDto -> UpdateCustomerCommand mapping
        CreateMap<UpdateCustomerDto, UpdateCustomerCommand>();
    }
}
