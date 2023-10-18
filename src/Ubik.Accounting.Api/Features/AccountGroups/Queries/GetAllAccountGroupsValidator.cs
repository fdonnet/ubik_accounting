using FluentValidation;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAllAccountGroups;

namespace Ubik.Accounting.Api.Features.AccountGroups.Queries
{
    public class GetAllAccountGroupsValidator : AbstractValidator<GetAllAccountGroupsQuery>
    {
        public GetAllAccountGroupsValidator()
        {

        }
    }
}
