using FluentValidation;
using Insurance.Application.Features.Policies.Commands;
using Insurance.Application.Features.Policies.Queries;

namespace Insurance.Application.Features.Policies.Validators;

public class CreatePolicyCommandValidator : AbstractValidator<CreatePolicyCommand>
{
    public CreatePolicyCommandValidator()
    {
        RuleFor(x => x.PolicyNumber).NotEmpty();
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.InsuranceProductId).GreaterThan(0);
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("StartDate must be before EndDate");
    }
}

public class UpdatePolicyCommandValidator : AbstractValidator<UpdatePolicyCommand>
{
    public UpdatePolicyCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class DeletePolicyCommandValidator : AbstractValidator<DeletePolicyCommand>
{
    public DeletePolicyCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class GetPolicyByIdQueryValidator : AbstractValidator<GetPolicyByIdQuery>
{
    public GetPolicyByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class GetAllPoliciesQueryValidator : AbstractValidator<GetAllPoliciesQuery>
{
    public GetAllPoliciesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
