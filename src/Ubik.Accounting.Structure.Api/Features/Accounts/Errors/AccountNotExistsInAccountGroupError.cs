using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.Accounts.Errors
{
    public class AccountNotExistsInAccountGroupError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountNotExistsInAccountGroupError(Guid id, Guid accountGroupId)
        {
            ErrorType = FeatureErrorType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_NOT_EXISTS_IN_ACCOUNTGROUP",
                ErrorFriendlyMessage = "The account doesn't exist in this account group.",
                ErrorValueDetails = $"Field:Id|AccountGroupId / Value:{id}|{accountGroupId}",
            }};
        }
    }
}
