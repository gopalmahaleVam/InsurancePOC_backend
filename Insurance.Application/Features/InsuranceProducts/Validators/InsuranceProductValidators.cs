using FluentValidation;
using Insurance.Application.Features.InsuranceProducts.Commands;
using Insurance.Application.Features.InsuranceProducts.Queries;

namespace Insurance.Application.Features.InsuranceProducts.Validators;

public class CreateInsuranceProductCommandValidator : AbstractValidator<CreateInsuranceProductCommand>
{
    public CreateInsuranceProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").MaximumLength(200);
        RuleFor(x => x.Type).NotEmpty().WithMessage("Type is required");
        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0);
    }
}

public class UpdateInsuranceProductCommandValidator : AbstractValidator<UpdateInsuranceProductCommand>
{
    public UpdateInsuranceProductCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        When(x => x.Name is not null, () => RuleFor(x => x.Name).NotEmpty().MaximumLength(200));
    }
}

public class DeleteInsuranceProductCommandValidator : AbstractValidator<DeleteInsuranceProductCommand>
{
    public DeleteInsuranceProductCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class GetInsuranceProductByIdQueryValidator : AbstractValidator<GetInsuranceProductByIdQuery>
{
    public GetInsuranceProductByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class GetAllInsuranceProductsQueryValidator : AbstractValidator<GetAllInsuranceProductsQuery>
{
    public GetAllInsuranceProductsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
