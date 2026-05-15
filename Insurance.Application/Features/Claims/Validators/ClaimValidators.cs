using FluentValidation;
using Insurance.Application.Features.Claims.Commands;
using Insurance.Application.Features.Claims.Queries;

namespace Insurance.Application.Features.Claims.Validators;

public class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator()
    {
        RuleFor(x => x.ClaimNumber).NotEmpty();
        RuleFor(x => x.PolicyId).GreaterThan(0);
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.ClaimAmount).GreaterThanOrEqualTo(0);
    }
}

public class UpdateClaimCommandValidator : AbstractValidator<UpdateClaimCommand>
{
    public UpdateClaimCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class DeleteClaimCommandValidator : AbstractValidator<DeleteClaimCommand>
{
    public DeleteClaimCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class GetClaimByIdQueryValidator : AbstractValidator<GetClaimByIdQuery>
{
    public GetClaimByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class GetAllClaimsQueryValidator : AbstractValidator<GetAllClaimsQuery>
{
    public GetAllClaimsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
