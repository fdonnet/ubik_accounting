using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public class AccountNotExistsInAccountGroup : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountNotExistsInAccountGroup(Guid id, Guid accountGroupId)
        {
            ErrorType = ServiceAndFeatureExceptionType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_NOT_EXISTS_IN_ACCOUNTGROUP",
                ErrorFriendlyMessage = "The account doesn't exist in this account group.",
                ErrorValueDetails = $"Field:Id|AccountGroupId / Value:{id}|{accountGroupId}",
            }};
        }
    }
}
