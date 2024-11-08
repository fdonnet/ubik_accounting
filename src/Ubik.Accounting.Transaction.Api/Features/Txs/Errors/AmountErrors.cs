using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Errors
{
    //TODO: do better error reporting (see when UI)
    public record AmountErrors : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AmountErrors(IEnumerable<SubmitTxEntry> entriesInError)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>();

            foreach (var entry in entriesInError)
            {
                CustomErrors.Add(new CustomError()
                {
                    ErrorCode = "BAD_AMOUNT_PARAMS_FOR_ENTRY",
                    ErrorFriendlyMessage = "This entry contains invalid amount value(s). Amount > 0",
                    ErrorValueDetails = $"Field:Amount / Value:{entry.Amount} - Field:OriginalAmount / Value:{entry.AmountAdditionnalInfo?.OriginalAmount}"
                });
            }
        }
    }
}
