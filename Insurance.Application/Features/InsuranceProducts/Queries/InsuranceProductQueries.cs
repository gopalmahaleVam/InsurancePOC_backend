using Insurance.Application.Common.Models;
using Insurance.Application.Features.InsuranceProducts.DTOs;
using MediatR;

namespace Insurance.Application.Features.InsuranceProducts.Queries;

public record GetInsuranceProductByIdQuery(int Id) : IRequest<Result<InsuranceProductResponseDto>>;

public record GetAllInsuranceProductsQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null, string? SortBy = null)
    : IRequest<Result<PaginatedResult<InsuranceProductResponseDto>>>;
