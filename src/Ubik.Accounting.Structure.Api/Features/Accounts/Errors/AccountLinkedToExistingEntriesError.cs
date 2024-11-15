using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.Accounts.Errors
{
    public class AccountLinkedToExistingEntriesError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountLinkedToExistingEntriesError(Guid id)
        {
            ErrorType = FeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new()
            {
                ErrorCode = "CANNOT_DELETE_ACCOUNT_LINKED_TO_EXISTING_ENTRIES",
                ErrorFriendlyMessage = "This account is linked to existing accounting entries. You can desactivate the account to be excluded in the future but cannot delete it.",
                ErrorValueDetails = $"Field:Id / Value:{id}",
            }};
        }
    }
}
