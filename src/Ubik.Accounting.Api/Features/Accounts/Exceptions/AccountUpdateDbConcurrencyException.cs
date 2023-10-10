using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public class AccountUpdateDbConcurrencyException : DbUpdateConcurrencyException, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public string ErrorFriendlyMessage { get; init; } = default!;
        public string ErrorCode { get; init; } = default!;
        public string ErrorValueDetails { get; init; } = default!;

        public AccountUpdateDbConcurrencyException(Guid version, DbUpdateConcurrencyException? innerException)
            : base("You don't have the last version or this account has been removed, refresh your data before updating."
                    , innerException)
        {
            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            ErrorCode = "ACCOUNT_MODIFIED";
            ErrorFriendlyMessage = "You don't have the last version or this account has been removed, refresh your data before updating.";
            ErrorValueDetails = $"Field:Version / Value:{version}";
        }
    }
}
