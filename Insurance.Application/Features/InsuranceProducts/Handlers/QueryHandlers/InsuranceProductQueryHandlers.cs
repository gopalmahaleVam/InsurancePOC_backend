using AutoMapper;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.InsuranceProducts.DTOs;
using Insurance.Application.Features.InsuranceProducts.Queries;
using Insurance.Application.Common.Interfaces;
using MediatR;

namespace Insurance.Application.Features.InsuranceProducts.Handlers.QueryHandlers;

public class GetInsuranceProductByIdHandler : IRequestHandler<GetInsuranceProductByIdQuery, Result<InsuranceProductResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetInsuranceProductByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<InsuranceProductResponseDto>> Handle(GetInsuranceProductByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result<InsuranceProductResponseDto>.Failure($"Product with ID {request.Id} not found");

        var dto = _mapper.Map<InsuranceProductResponseDto>(entity);
        return Result<InsuranceProductResponseDto>.Success(dto);
    }
}

public class GetAllInsuranceProductsHandler : IRequestHandler<GetAllInsuranceProductsQuery, Result<PaginatedResult<InsuranceProductResponseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllInsuranceProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<InsuranceProductResponseDto>>> Handle(GetAllInsuranceProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _unitOfWork.Products.GetPaginatedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var dtos = items.Select(i => _mapper.Map<InsuranceProductResponseDto>(i)).ToList();
        var paginated = new PaginatedResult<InsuranceProductResponseDto>(dtos, request.PageNumber, request.PageSize, total);
        return Result<PaginatedResult<InsuranceProductResponseDto>>.Success(paginated);
    }
}
