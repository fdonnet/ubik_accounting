using FluentValidation;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAccountGroup;

namespace Ubik.Accounting.Api.Features.AccountGroups.Queries
{
    public class GetAccountGroupValidator : AbstractValidator<GetAccountGroupQuery>
    {
        public GetAccountGroupValidator()
        {
            RuleFor(query => query.Id)
               .NotEmpty().WithMessage("Id required");
        }
    }
}
