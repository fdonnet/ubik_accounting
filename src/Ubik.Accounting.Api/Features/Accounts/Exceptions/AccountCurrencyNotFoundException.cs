using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public class AccountCurrencyNotFoundException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountCurrencyNotFoundException(Guid idNotFound)
         : base($"The currency id specified: {idNotFound} is not found.")
        {
            ErrorType = ServiceAndFeatureExceptionType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_CURRENCY_NOT_FOUND",
                ErrorFriendlyMessage = "The currency specified doesn't exist. Id not found.",
                ErrorValueDetails = $"Field:CurrencyId / Value:{idNotFound}",
            }};
        }
    }
}
