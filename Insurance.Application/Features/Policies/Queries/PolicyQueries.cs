using Insurance.Application.Common.Models;
using Insurance.Application.Features.Policies.DTOs;
using MediatR;

namespace Insurance.Application.Features.Policies.Queries;

public record GetPolicyByIdQuery(int Id) : IRequest<Result<PolicyResponseDto>>;

public record GetAllPoliciesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResult<PolicyResponseDto>>>;
