using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Claims.DTOs;
using Insurance.Application.Features.Claims.Queries;
using MediatR;

namespace Insurance.Application.Features.Claims.Handlers.QueryHandlers;

public class GetClaimByIdHandler : IRequestHandler<GetClaimByIdQuery, Result<ClaimResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetClaimByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ClaimResponseDto>> Handle(GetClaimByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Claims.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<ClaimResponseDto>.Failure($"Claim with ID {request.Id} not found");
        return Result<ClaimResponseDto>.Success(_mapper.Map<ClaimResponseDto>(entity));
    }
}

public class GetAllClaimsHandler : IRequestHandler<GetAllClaimsQuery, Result<PaginatedResult<ClaimResponseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllClaimsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<ClaimResponseDto>>> Handle(GetAllClaimsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _unitOfWork.Claims.GetPaginatedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var dtos = items.Select(i => _mapper.Map<ClaimResponseDto>(i)).ToList();
        var paginated = new PaginatedResult<ClaimResponseDto>(dtos, request.PageNumber, request.PageSize, total);
        return Result<PaginatedResult<ClaimResponseDto>>.Success(paginated);
    }
}
