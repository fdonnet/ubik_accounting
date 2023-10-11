using FluentValidation;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;

namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    public class GetAccountValidator : AbstractValidator<GetAccountQuery>
    {
        public GetAccountValidator()
        {
            RuleFor(query => query.Id)
               .NotEmpty().WithMessage("Id required");
        }
    }
}
