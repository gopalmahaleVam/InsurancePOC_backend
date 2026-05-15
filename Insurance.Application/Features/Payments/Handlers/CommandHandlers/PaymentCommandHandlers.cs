using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Payments.Commands;
using Insurance.Application.Features.Payments.DTOs;
using Insurance.Domain.Entities;
using MediatR;

namespace Insurance.Application.Features.Payments.Handlers.CommandHandlers;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<PaymentResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePaymentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaymentResponseDto>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = new Payment
        {
            TransactionId = request.TransactionId,
            PolicyId = request.PolicyId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            PaymentDate = request.PaymentDate,
            PaymentMethod = request.PaymentMethod,
            Description = request.Description
        };

        try
        {
            await _unitOfWork.Payments.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<PaymentResponseDto>.Success(_mapper.Map<PaymentResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return Result<PaymentResponseDto>.Failure($"Failed to create payment: {ex.Message}");
        }
    }
}

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, Result<PaymentResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdatePaymentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaymentResponseDto>> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Payments.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<PaymentResponseDto>.Failure($"Payment with ID {request.Id} not found");

        if (!string.IsNullOrEmpty(request.TransactionId)) entity.TransactionId = request.TransactionId;
        if (request.PolicyId.HasValue) entity.PolicyId = request.PolicyId.Value;
        if (request.CustomerId.HasValue) entity.CustomerId = request.CustomerId.Value;
        if (request.Amount.HasValue) entity.Amount = request.Amount.Value;
        if (request.PaymentDate.HasValue) entity.PaymentDate = request.PaymentDate.Value;
        if (request.PaymentMethod is not null) entity.PaymentMethod = request.PaymentMethod;
        if (request.Status is not null) entity.Status = request.Status;
        if (request.Description is not null) entity.Description = request.Description;

        entity.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.Payments.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<PaymentResponseDto>.Success(_mapper.Map<PaymentResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return Result<PaymentResponseDto>.Failure($"Failed to update payment: {ex.Message}");
        }
    }
}

public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePaymentCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _unitOfWork.Payments.DeleteAsync(request.Id, cancellationToken);
            if (!deleted) return Result<bool>.Failure($"Payment with ID {request.Id} not found");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete payment: {ex.Message}");
        }
    }
}
