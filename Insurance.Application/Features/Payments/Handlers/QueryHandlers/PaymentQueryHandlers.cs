using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Payments.DTOs;
using Insurance.Application.Features.Payments.Queries;
using MediatR;

namespace Insurance.Application.Features.Payments.Handlers.QueryHandlers;

public class GetPaymentByIdHandler : IRequestHandler<GetPaymentByIdQuery, Result<PaymentResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPaymentByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaymentResponseDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Payments.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<PaymentResponseDto>.Failure($"Payment with ID {request.Id} not found");
        return Result<PaymentResponseDto>.Success(_mapper.Map<PaymentResponseDto>(entity));
    }
}

public class GetAllPaymentsHandler : IRequestHandler<GetAllPaymentsQuery, Result<PaginatedResult<PaymentResponseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPaymentsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<PaymentResponseDto>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _unitOfWork.Payments.GetPaginatedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var dtos = items.Select(i => _mapper.Map<PaymentResponseDto>(i)).ToList();
        var paginated = new PaginatedResult<PaymentResponseDto>(dtos, request.PageNumber, request.PageSize, total);
        return Result<PaginatedResult<PaymentResponseDto>>.Success(paginated);
    }
}
