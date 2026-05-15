using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Policies.DTOs;
using Insurance.Application.Features.Policies.Queries;
using MediatR;

namespace Insurance.Application.Features.Policies.Handlers.QueryHandlers;

public class GetPolicyByIdHandler : IRequestHandler<GetPolicyByIdQuery, Result<PolicyResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPolicyByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PolicyResponseDto>> Handle(GetPolicyByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Policies.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<PolicyResponseDto>.Failure($"Policy with ID {request.Id} not found");
        return Result<PolicyResponseDto>.Success(_mapper.Map<PolicyResponseDto>(entity));
    }
}

public class GetAllPoliciesHandler : IRequestHandler<GetAllPoliciesQuery, Result<PaginatedResult<PolicyResponseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPoliciesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<PolicyResponseDto>>> Handle(GetAllPoliciesQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _unitOfWork.Policies.GetPaginatedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var dtos = items.Select(i => _mapper.Map<PolicyResponseDto>(i)).ToList();
        var paginated = new PaginatedResult<PolicyResponseDto>(dtos, request.PageNumber, request.PageSize, total);
        return Result<PaginatedResult<PolicyResponseDto>>.Success(paginated);
    }
}
