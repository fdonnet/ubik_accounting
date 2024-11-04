using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountLinkedTaxRates.Errors
{
    public record LinkedTaxRateNotFoundError : IServiceAndFeatureError
    {
          public ServiceAndFeatureErrorType ErrorType { get; init; }
            public List<CustomError> CustomErrors { get; init; }

            public LinkedTaxRateNotFoundError(Guid accountId, Guid taxRateId)
            {

                ErrorType = ServiceAndFeatureErrorType.NotFound;
                CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "LINKED_TAX_RATE_NOT_FOUND",
                ErrorFriendlyMessage = "The tax rate for this account is not found",
                ErrorValueDetails = $"Field:AccountId / Value:{accountId} - Field:TaxRateId / Value:{taxRateId}"
            }};
            }

    }
}
