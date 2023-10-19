using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public class AccountGroupNotFoundExceptionForAccount : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupNotFoundExceptionForAccount(Guid accountGroupIdNotFound)
         : base($"An account group with this id: {accountGroupIdNotFound} is not found and cannot be linked to an account.")
        {
            ErrorType = ServiceAndFeatureExceptionType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_NOT_FOUND_FOR_ACCOUNT",
                ErrorFriendlyMessage = "The account group doesn't exist. Id not found.",
                ErrorValueDetails = $"Field:AccountGroupId / Value:{accountGroupIdNotFound}",
            }};
        }
    }
}
