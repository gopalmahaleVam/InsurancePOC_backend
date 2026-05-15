using FluentValidation;
using Insurance.Application.Features.Payments.Commands;
using Insurance.Application.Features.Payments.Queries;

namespace Insurance.Application.Features.Payments.Validators;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.TransactionId).NotEmpty();
        RuleFor(x => x.PolicyId).GreaterThan(0);
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class UpdatePaymentCommandValidator : AbstractValidator<UpdatePaymentCommand>
{
    public UpdatePaymentCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class DeletePaymentCommandValidator : AbstractValidator<DeletePaymentCommand>
{
    public DeletePaymentCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class GetPaymentByIdQueryValidator : AbstractValidator<GetPaymentByIdQuery>
{
    public GetPaymentByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class GetAllPaymentsQueryValidator : AbstractValidator<GetAllPaymentsQuery>
{
    public GetAllPaymentsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
