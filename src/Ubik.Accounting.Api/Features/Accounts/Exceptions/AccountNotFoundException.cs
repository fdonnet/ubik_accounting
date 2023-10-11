using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public class AccountNotFoundException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountNotFoundException(Guid idNotFound)
         : base($"An account with this id: {idNotFound} is not found.")
        {
            ErrorType = ServiceAndFeatureExceptionType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_NOT_FOUND",
                ErrorFriendlyMessage = "The account doesn't exist. Id not found.",
                ErrorValueDetails = $"Field:Id / Value:{idNotFound}",
            }};
        }
    }
}
