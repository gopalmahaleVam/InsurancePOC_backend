using Insurance.Application.Common.Models;
using Insurance.Application.Features.Payments.DTOs;
using MediatR;

namespace Insurance.Application.Features.Payments.Queries;

public record GetPaymentByIdQuery(int Id) : IRequest<Result<PaymentResponseDto>>;

public record GetAllPaymentsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResult<PaymentResponseDto>>>;
