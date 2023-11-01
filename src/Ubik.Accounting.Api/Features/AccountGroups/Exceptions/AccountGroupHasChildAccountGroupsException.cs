using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public record AccountGroupHasChildAccountGroupsException : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupHasChildAccountGroupsException(Guid id)
        {

            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_HAS_CHILD_ACCOUNTGROUPS",
                ErrorFriendlyMessage = "This account group (id) has child accounts groups.",
                ErrorValueDetails = $"Field:Id / Value:{id}"
            }};
        }
    }
}
