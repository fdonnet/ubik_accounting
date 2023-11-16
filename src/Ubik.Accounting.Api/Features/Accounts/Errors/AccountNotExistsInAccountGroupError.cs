using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Errors
{
    public class AccountNotExistsInAccountGroupError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountNotExistsInAccountGroupError(Guid id, Guid accountGroupId)
        {
            ErrorType = ServiceAndFeatureErrorType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_NOT_EXISTS_IN_ACCOUNTGROUP",
                ErrorFriendlyMessage = "The account doesn't exist in this account group.",
                ErrorValueDetails = $"Field:Id|AccountGroupId / Value:{id}|{accountGroupId}",
            }};
        }
    }
}
