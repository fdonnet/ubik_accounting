using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public record AccountCurrencyNotFoundException : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountCurrencyNotFoundException(Guid idNotFound)
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
