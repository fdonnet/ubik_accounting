using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public class AccountAlreadyExistsInClassificationException : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountAlreadyExistsInClassificationException(Guid id, Guid accountGroupId)
        {
            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_ALREADY_EXISTS_IN_CLASSIFICATION",
                ErrorFriendlyMessage = "This account already exists in the classification.",
                ErrorValueDetails = $"Field:Id|AccountGroupId / Value:{id}|{accountGroupId}",
            }};
        }
    }
}
