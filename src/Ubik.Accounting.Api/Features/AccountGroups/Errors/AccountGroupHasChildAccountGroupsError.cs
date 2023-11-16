using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Errors
{
    public record AccountGroupHasChildAccountGroupsError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupHasChildAccountGroupsError(Guid id)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_HAS_CHILD_ACCOUNTGROUPS",
                ErrorFriendlyMessage = "This account group (id) has child accounts groups.",
                ErrorValueDetails = $"Field:Id / Value:{id}"
            }};
        }
    }
}
