using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.AccountTaxRateConfigs.Errors
{
    public record LinkedTaxRateAlreadyExist : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public LinkedTaxRateAlreadyExist(Guid accountId, Guid taxRateId)
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "LINKED_TAX_RATE_ALREADY_EXIST",
                ErrorFriendlyMessage = "The tax rate for this account is already attached",
                ErrorValueDetails = $"Field:AccountId / Value:{accountId} - Field:TaxRateId / Value:{taxRateId}"
            }};
        }
    }
}
