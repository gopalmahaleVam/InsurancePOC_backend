using AutoMapper;
using Insurance.Application.Common.Interfaces;
using Insurance.Application.Common.Models;
using Insurance.Application.Features.Customers.DTOs;
using Insurance.Application.Features.Customers.Queries;
using MediatR;

namespace Insurance.Application.Features.Customers.Handlers.QueryHandlers;

/// <summary>
/// Query handlers for customer-related queries using CQRS pattern.
/// </summary>
public class CustomerQueryHandlers :
    IRequestHandler<GetAllCustomersQuery, PaginatedResult<CustomerListDto>>,
    IRequestHandler<GetCustomerByIdQuery, Result<CustomerResponseDto?>>,
    IRequestHandler<SearchCustomersByNameQuery, Result<IEnumerable<CustomerListDto>>>,
    IRequestHandler<GetCustomersByCityQuery, Result<IEnumerable<CustomerListDto>>>,
    IRequestHandler<GetCustomersByStateQuery, Result<IEnumerable<CustomerListDto>>>,
    IRequestHandler<GetCustomersByZipCodeQuery, Result<IEnumerable<CustomerListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerQueryHandlers(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Handles GetAllCustomersQuery to retrieve paginated list of customers.
    /// </summary>
    public async Task<PaginatedResult<CustomerListDto>> Handle(
        GetAllCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Customers.GetPaginatedAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var customerDtos = _mapper.Map<List<CustomerListDto>>(items);
        return new PaginatedResult<CustomerListDto>(
            customerDtos,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }

    /// <summary>
    /// Handles GetCustomerByIdQuery to retrieve a specific customer by ID.
    /// </summary>
    public async Task<Result<CustomerResponseDto?>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer == null)
        {
            return Result<CustomerResponseDto?>.Failure(
                $"Customer with ID {request.CustomerId} not found");
        }

        var customerDto = _mapper.Map<CustomerResponseDto>(customer);
        return Result<CustomerResponseDto?>.Success(customerDto);
    }

    /// <summary>
    /// Handles SearchCustomersByNameQuery to search for customers by first or last name.
    /// </summary>
    public async Task<Result<IEnumerable<CustomerListDto>>> Handle(
        SearchCustomersByNameQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return Result<IEnumerable<CustomerListDto>>.Failure(
                "Search term cannot be empty");
        }

        var customers = await _unitOfWork.Customers.SearchByNameAsync(
            request.SearchTerm,
            cancellationToken);

        var customerDtos = _mapper.Map<List<CustomerListDto>>(customers);
        return Result<IEnumerable<CustomerListDto>>.Success(customerDtos);
    }

    /// <summary>
    /// Handles GetCustomersByCityQuery to retrieve all customers in a specific city.
    /// </summary>
    public async Task<Result<IEnumerable<CustomerListDto>>> Handle(
        GetCustomersByCityQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.City))
        {
            return Result<IEnumerable<CustomerListDto>>.Failure(
                "City cannot be empty");
        }

        var customers = await _unitOfWork.Customers.GetByCityAsync(
            request.City,
            cancellationToken);

        var customerDtos = _mapper.Map<List<CustomerListDto>>(customers);
        return Result<IEnumerable<CustomerListDto>>.Success(customerDtos);
    }

    /// <summary>
    /// Handles GetCustomersByStateQuery to retrieve all customers in a specific state.
    /// </summary>
    public async Task<Result<IEnumerable<CustomerListDto>>> Handle(
        GetCustomersByStateQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.State))
        {
            return Result<IEnumerable<CustomerListDto>>.Failure(
                "State cannot be empty");
        }

        var customers = await _unitOfWork.Customers.GetByStateAsync(
            request.State,
            cancellationToken);

        var customerDtos = _mapper.Map<List<CustomerListDto>>(customers);
        return Result<IEnumerable<CustomerListDto>>.Success(customerDtos);
    }

    /// <summary>
    /// Handles GetCustomersByZipCodeQuery to retrieve all customers with a specific zip code.
    /// </summary>
    public async Task<Result<IEnumerable<CustomerListDto>>> Handle(
        GetCustomersByZipCodeQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ZipCode))
        {
            return Result<IEnumerable<CustomerListDto>>.Failure(
                "Zip code cannot be empty");
        }

        var customers = await _unitOfWork.Customers.GetByZipCodeAsync(
            request.ZipCode,
            cancellationToken);

        var customerDtos = _mapper.Map<List<CustomerListDto>>(customers);
        return Result<IEnumerable<CustomerListDto>>.Success(customerDtos);
    }
}
