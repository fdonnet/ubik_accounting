using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public class AccountAlreadyExistsException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public string ErrorFriendlyMessage { get; init; } = default!;
        public string ErrorCode { get; init; } = default!;
        public string ErrorValueDetails { get; init; } = default!;

        public AccountAlreadyExistsException(string codeAlreadyExisting)
         : base($"An account with this code: {codeAlreadyExisting} already exists.")
        {
            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            ErrorCode = "ACCOUNT_ALREADY_EXISTS";
            ErrorFriendlyMessage = "The account already exists. Code field needs to be unique.";
            ErrorValueDetails = $"Field:Code / Value:{codeAlreadyExisting}";
        }
    }
}
