using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Errors
{
    public record AccountAlreadyExistsError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountAlreadyExistsError(string codeAlreadyExisting)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_ALREADY_EXISTS",
                ErrorFriendlyMessage = "The account already exists. Code field needs to be unique.",
                ErrorValueDetails = $"Field:Code / Value:{codeAlreadyExisting}"
            }};
        }
    }
}
