using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Errors
{
    public record AccountGroupHasChildAccountsError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupHasChildAccountsError(Guid id)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_HAS_CHILD_ACCOUNTS",
                ErrorFriendlyMessage = "This account group (id) has child accounts.",
                ErrorValueDetails = $"Field:Id / Value:{id}"
            }};
        }
    }
}
