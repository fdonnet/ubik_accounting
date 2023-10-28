using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public class AccountAlreadyExistsException : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountAlreadyExistsException(string codeAlreadyExisting)
        {

            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_ALREADY_EXISTS",
                ErrorFriendlyMessage = "The account already exists. Code field needs to be unique.",
                ErrorValueDetails = $"Field:Code / Value:{codeAlreadyExisting}"
            }};
        }
    }
}
