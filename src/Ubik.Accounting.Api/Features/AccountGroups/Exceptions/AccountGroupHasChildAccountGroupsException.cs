using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public class AccountGroupHasChildAccountGroupsException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupHasChildAccountGroupsException(Guid id)
         : base($"This account group has child account groups, you cannot delete it without removing its children first.")
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
