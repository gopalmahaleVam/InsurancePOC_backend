using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Claims.Commands;
using Insurance.Application.Features.Claims.DTOs;
using Insurance.Domain.Entities;
using MediatR;

namespace Insurance.Application.Features.Claims.Handlers.CommandHandlers;

public class CreateClaimCommandHandler : IRequestHandler<CreateClaimCommand, Result<ClaimResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ClaimResponseDto>> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
    {
        var entity = new Claim
        {
            ClaimNumber = request.ClaimNumber,
            PolicyId = request.PolicyId,
            CustomerId = request.CustomerId,
            ClaimDate = request.ClaimDate,
            Description = request.Description,
            ClaimAmount = request.ClaimAmount
        };

        try
        {
            await _unitOfWork.Claims.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<ClaimResponseDto>.Success(_mapper.Map<ClaimResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return Result<ClaimResponseDto>.Failure($"Failed to create claim: {ex.Message}");
        }
    }
}

public class UpdateClaimCommandHandler : IRequestHandler<UpdateClaimCommand, Result<ClaimResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ClaimResponseDto>> Handle(UpdateClaimCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Claims.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return Result<ClaimResponseDto>.Failure($"Claim with ID {request.Id} not found");

        if (!string.IsNullOrEmpty(request.ClaimNumber)) entity.ClaimNumber = request.ClaimNumber;
        if (request.PolicyId.HasValue) entity.PolicyId = request.PolicyId.Value;
        if (request.CustomerId.HasValue) entity.CustomerId = request.CustomerId.Value;
        if (request.ClaimDate.HasValue) entity.ClaimDate = request.ClaimDate.Value;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.ClaimAmount.HasValue) entity.ClaimAmount = request.ClaimAmount.Value;
        if (request.Status.HasValue) entity.Status = request.Status.Value;
        if (request.ResolutionDate.HasValue) entity.ResolutionDate = request.ResolutionDate.Value;
        if (request.ResolutionNotes is not null) entity.ResolutionNotes = request.ResolutionNotes;

        entity.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.Claims.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<ClaimResponseDto>.Success(_mapper.Map<ClaimResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return Result<ClaimResponseDto>.Failure($"Failed to update claim: {ex.Message}");
        }
    }
}

public class DeleteClaimCommandHandler : IRequestHandler<DeleteClaimCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClaimCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(DeleteClaimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _unitOfWork.Claims.DeleteAsync(request.Id, cancellationToken);
            if (!deleted) return Result<bool>.Failure($"Claim with ID {request.Id} not found");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete claim: {ex.Message}");
        }
    }
}
