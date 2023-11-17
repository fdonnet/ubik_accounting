using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Errors
{
    public class AccountAlreadyExistsInClassificationError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountAlreadyExistsInClassificationError(Guid id, Guid accountGroupId)
        {
            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_ALREADY_EXISTS_IN_CLASSIFICATION",
                ErrorFriendlyMessage = "This account already exists in the classification.",
                ErrorValueDetails = $"Field:Id|AccountGroupId / Value:{id}|{accountGroupId}",
            }};
        }
    }
}
