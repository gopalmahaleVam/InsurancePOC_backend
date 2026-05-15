using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Customers.Commands;
using Insurance.Application.Features.Customers.DTOs;
using Insurance.Domain.Entities;
using MediatR;

namespace Insurance.Application.Features.Customers.Handlers.CommandHandlers;

/// <summary>
/// Command handlers for customer-related commands using CQRS pattern.
/// Handles Create, Update, and Delete operations for customers.
/// </summary>
public class CustomerCommandHandlers :
    IRequestHandler<CreateCustomerCommand, Result<CustomerResponseDto>>,
    IRequestHandler<UpdateCustomerCommand, Result<CustomerResponseDto>>,
    IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerCommandHandlers(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Handles CreateCustomerCommand to create a new customer in the system.
    /// Validates that email doesn't already exist and then creates the customer.
    /// </summary>
    public async Task<Result<CustomerResponseDto>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        // Check if email already exists
        var emailExists = await _unitOfWork.Customers.EmailExistsAsync(
            request.Email,
            cancellationToken);

        if (emailExists)
        {
            return Result<CustomerResponseDto>.Failure(
                $"A customer with email '{request.Email}' already exists");
        }

        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var customerDto = _mapper.Map<CustomerResponseDto>(customer);
        return Result<CustomerResponseDto>.Success(customerDto);
    }

    /// <summary>
    /// Handles UpdateCustomerCommand to update an existing customer's information.
    /// Validates that the customer exists and that email is unique (if changed).
    /// </summary>
    public async Task<Result<CustomerResponseDto>> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer == null)
        {
            return Result<CustomerResponseDto>.Failure(
                $"Customer with ID {request.CustomerId} not found");
        }

        // Check if email is being changed and if the new email already exists
        if (!customer.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _unitOfWork.Customers.EmailExistsAsync(
                request.Email,
                cancellationToken);

            if (emailExists)
            {
                return Result<CustomerResponseDto>.Failure(
                    $"Email '{request.Email}' is already in use by another customer");
            }
        }

        // Update customer properties
        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.PhoneNumber = request.PhoneNumber;
        customer.Address = request.Address;
        customer.City = request.City;
        customer.State = request.State;
        customer.ZipCode = request.ZipCode;
        customer.DateOfBirth = request.DateOfBirth;
        customer.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Customers.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var customerDto = _mapper.Map<CustomerResponseDto>(customer);
        return Result<CustomerResponseDto>.Success(customerDto);
    }

    /// <summary>
    /// Handles DeleteCustomerCommand to soft-delete a customer from the system.
    /// The customer data is retained in the database but marked as deleted.
    /// </summary>
    public async Task<Result> Handle(
        DeleteCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer == null)
        {
            return Result.FailureResult(
                $"Customer with ID {request.CustomerId} not found");
        }

        // Soft delete: mark as deleted instead of removing from database
        await _unitOfWork.Customers.DeleteAsync(request.CustomerId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("Customer deleted successfully");
    }
}
