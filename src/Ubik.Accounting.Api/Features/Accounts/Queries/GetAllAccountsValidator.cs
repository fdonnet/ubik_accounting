using FluentValidation;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAllAccounts;

namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    public class GetAllAccountsValidator : AbstractValidator<GetAllAccountsQuery>
    {
        public GetAllAccountsValidator()
        {

        }
    }
}
