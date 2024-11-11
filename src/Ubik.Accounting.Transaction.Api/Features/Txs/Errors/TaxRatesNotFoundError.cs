using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Errors
{
    public record TaxRatesNotFoundError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public TaxRatesNotFoundError(IEnumerable<SubmitTxEntry> entriesInError)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>();

            foreach (var entry in entriesInError)
            {
                CustomErrors.Add(new CustomError()
                {
                    ErrorCode = "TAX_RATE_NOT_FOUND_FOR_ENTRY",
                    ErrorFriendlyMessage = "The specified tax rate is not found.",
                    ErrorValueDetails = $"Field:TaxInfo.TaxRateId / Value:{entry.TaxInfo!.TaxRateId}"
                });
            }
        }
    }
}
