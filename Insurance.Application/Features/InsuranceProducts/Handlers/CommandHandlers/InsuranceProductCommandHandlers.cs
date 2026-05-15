using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.InsuranceProducts.Commands;
using Insurance.Application.Features.InsuranceProducts.DTOs;
using Insurance.Domain.Entities;
using MediatR;

namespace Insurance.Application.Features.InsuranceProducts.Handlers.CommandHandlers;

public class CreateInsuranceProductCommandHandler : IRequestHandler<CreateInsuranceProductCommand, Result<InsuranceProductResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateInsuranceProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<InsuranceProductResponseDto>> Handle(CreateInsuranceProductCommand request, CancellationToken cancellationToken)
    {
        var entity = new InsuranceProduct
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Type = request.Type,
            BasePrice = request.BasePrice,
            CoverageDetails = request.CoverageDetails ?? string.Empty,
            CoverageLimitInDays = request.CoverageLimitInDays
        };

        try
        {
            await _unitOfWork.Products.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var dto = _mapper.Map<InsuranceProductResponseDto>(entity);
            return Result<InsuranceProductResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<InsuranceProductResponseDto>.Failure($"Failed to create product: {ex.Message}");
        }
    }
}

public class UpdateInsuranceProductCommandHandler : IRequestHandler<UpdateInsuranceProductCommand, Result<InsuranceProductResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateInsuranceProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<InsuranceProductResponseDto>> Handle(UpdateInsuranceProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result<InsuranceProductResponseDto>.Failure($"Product with ID {request.Id} not found");

        if (!string.IsNullOrEmpty(request.Name)) entity.Name = request.Name;
        if (request.Description is not null) entity.Description = request.Description;
        if (!string.IsNullOrEmpty(request.Type)) entity.Type = request.Type;
        if (request.BasePrice.HasValue) entity.BasePrice = request.BasePrice.Value;
        if (request.CoverageDetails is not null) entity.CoverageDetails = request.CoverageDetails;
        if (request.CoverageLimitInDays.HasValue) entity.CoverageLimitInDays = request.CoverageLimitInDays.Value;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.Products.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var dto = _mapper.Map<InsuranceProductResponseDto>(entity);
            return Result<InsuranceProductResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<InsuranceProductResponseDto>.Failure($"Failed to update product: {ex.Message}");
        }
    }
}

public class DeleteInsuranceProductCommandHandler : IRequestHandler<DeleteInsuranceProductCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInsuranceProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteInsuranceProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _unitOfWork.Products.DeleteAsync(request.Id, cancellationToken);
            if (!deleted) return Result<bool>.Failure($"Product with ID {request.Id} not found");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to delete product: {ex.Message}");
        }
    }
}
