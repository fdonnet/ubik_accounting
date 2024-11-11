using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Errors
{
    public record TaxRatesNotMatchError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public TaxRatesNotMatchError(IEnumerable<SubmitTxEntry> entriesInError)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>();

            foreach (var entry in entriesInError)
            {
                CustomErrors.Add(new CustomError()
                {
                    ErrorCode = "TAX_RATE_NOT_MATCH_FOR_ENTRY",
                    ErrorFriendlyMessage = "The tax rate provided not match the actual configuration.",
                    ErrorValueDetails = $"Field:TaxInfo.TaxRateId / Value:{entry.TaxInfo!.TaxRateId} - Field:TaxInfo.TaxAppliedRate / Value:{entry.TaxInfo!.TaxAppliedRate}"
                });
            }
        }
    }
}
