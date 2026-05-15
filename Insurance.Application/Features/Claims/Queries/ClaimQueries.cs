using Insurance.Application.Common.Models;
using Insurance.Application.Features.Claims.DTOs;
using MediatR;

namespace Insurance.Application.Features.Claims.Queries;

public record GetClaimByIdQuery(int Id) : IRequest<Result<ClaimResponseDto>>;

public record GetAllClaimsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResult<ClaimResponseDto>>>;
