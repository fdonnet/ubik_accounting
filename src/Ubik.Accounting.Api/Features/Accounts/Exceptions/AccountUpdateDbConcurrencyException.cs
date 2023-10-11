using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public class AccountUpdateDbConcurrencyException : DbUpdateConcurrencyException, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountUpdateDbConcurrencyException(Guid version, DbUpdateConcurrencyException? innerException)
            : base("You don't have the last version or this account has been removed, refresh your data before updating."
                    , innerException)
        {
            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_MODIFIED",
                ErrorFriendlyMessage = "You don't have the last version or this account has been removed, refresh your data before updating.",
                ErrorValueDetails = $"Field:Version / Value:{version}",
            }};
        }
    }
}
