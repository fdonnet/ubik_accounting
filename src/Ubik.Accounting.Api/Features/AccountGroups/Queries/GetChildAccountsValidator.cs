using FluentValidation;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetChildAccounts;

namespace Ubik.Accounting.Api.Features.AccountGroups.Queries
{
    public class GetChildAccountsValidator : AbstractValidator<GetChildAccountsQuery>
    {
        public GetChildAccountsValidator()
        {
            RuleFor(query => query.AccountGroupId)
               .NotEmpty().WithMessage("AccountGroupId required");
        }
    }
}
