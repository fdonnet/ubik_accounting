using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.Accounts.Errors
{
    public class AccountAlreadyExistsInClassificationError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountAlreadyExistsInClassificationError(Guid id, Guid accountGroupId)
        {
            ErrorType = FeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new()
            {
                ErrorCode = "ACCOUNT_ALREADY_EXISTS_IN_CLASSIFICATION",
                ErrorFriendlyMessage = "This account already exists in the classification.",
                ErrorValueDetails = $"Field:Id|AccountGroupId / Value:{id}|{accountGroupId}",
            }};
        }
    }
}
