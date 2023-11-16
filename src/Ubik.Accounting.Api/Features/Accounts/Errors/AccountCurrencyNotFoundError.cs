using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Errors
{
    public record AccountCurrencyNotFoundError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountCurrencyNotFoundError(Guid idNotFound)
        {
            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_CURRENCY_NOT_FOUND",
                ErrorFriendlyMessage = "The currency specified doesn't exist. Id not found.",
                ErrorValueDetails = $"Field:CurrencyId / Value:{idNotFound}",
            }};
        }
    }
}
