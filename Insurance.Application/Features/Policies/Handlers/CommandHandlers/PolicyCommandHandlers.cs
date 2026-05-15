using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Policies.Commands;
using Insurance.Application.Features.Policies.DTOs;
using Insurance.Domain.Entities;
using MediatR;

namespace Insurance.Application.Features.Policies.Handlers.CommandHandlers;

public class CreatePolicyCommandHandler : IRequestHandler<CreatePolicyCommand, Result<PolicyResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePolicyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PolicyResponseDto>> Handle(CreatePolicyCommand request, CancellationToken cancellationToken)
    {
        var entity = new Policy
        {
            PolicyNumber = request.PolicyNumber,
            CustomerId = request.CustomerId,
            InsuranceProductId = request.InsuranceProductId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            PremiumAmount = request.PremiumAmount,
            PaymentFrequency = request.PaymentFrequency ?? "Annual"
        };

        try
        {
            await _unitOfWork.Policies.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<PolicyResponseDto>.Success(_mapper.Map<PolicyResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return Result<PolicyResponseDto>.Failure($"Failed to create policy: {ex.Message}");
        }
    }
}

public class UpdatePolicyCommandHandler : IRequestHandler<UpdatePolicyCommand, Result<PolicyResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdatePolicyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PolicyResponseDto>> Handle(UpdatePolicyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Policies.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<PolicyResponseDto>.Failure($"Policy with ID {request.Id} not found");

        if (!string.IsNullOrEmpty(request.PolicyNumber)) entity.PolicyNumber = request.PolicyNumber;
        if (request.CustomerId.HasValue) entity.CustomerId = request.CustomerId.Value;
        if (request.InsuranceProductId.HasValue) entity.InsuranceProductId = request.InsuranceProductId.Value;
        if (request.StartDate.HasValue) entity.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) entity.EndDate = request.EndDate.Value;
        if (request.PremiumAmount.HasValue) entity.PremiumAmount = request.PremiumAmount.Value;
        if (request.PaymentFrequency is not null) entity.PaymentFrequency = request.PaymentFrequency;
        if (request.Status.HasValue) entity.Status = request.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.Policies.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<PolicyResponseDto>.Success(_mapper.Map<PolicyResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return Result<PolicyResponseDto>.Failure($"Failed to update policy: {ex.Message}");
        }
    }
}

public class DeletePolicyCommandHandler : IRequestHandler<DeletePolicyCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePolicyCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(DeletePolicyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _unitOfWork.Policies.DeleteAsync(request.Id, cancellationToken);
            if (!deleted) return Result<bool>.Failure($"Policy with ID {request.Id} not found");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete policy: {ex.Message}");
        }
    }
}
