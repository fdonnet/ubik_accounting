using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountTaxRateConfigs.Errors
{
    public record AccountTaxRateConfigNotFoundError : IFeatureError
    {
          public FeatureErrorType ErrorType { get; init; }
            public List<CustomError> CustomErrors { get; init; }

            public AccountTaxRateConfigNotFoundError(Guid accountId, Guid taxRateId)
            {

                ErrorType = FeatureErrorType.NotFound;
                CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "LINKED_TAX_RATE_NOT_FOUND",
                ErrorFriendlyMessage = "The tax rate for this account is not found",
                ErrorValueDetails = $"Field:AccountId / Value:{accountId} - Field:TaxRateId / Value:{taxRateId}"
            }};
            }

    }
}
