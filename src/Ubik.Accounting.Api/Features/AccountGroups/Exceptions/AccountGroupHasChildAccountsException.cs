using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public class AccountGroupHasChildAccountsException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupHasChildAccountsException(Guid id)
         : base($"This account group has child accounts, you cannot delete it without removing its children first.")
        {

            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_HAS_CHILD_ACCOUNTS",
                ErrorFriendlyMessage = "This account group (id) has child accounts.",
                ErrorValueDetails = $"Field:Id / Value:{id}"
            }};
        }
    }
}
