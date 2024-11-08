using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Errors
{
    public class BalanceError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public BalanceError(decimal amount, decimal debitAmount, decimal creditAmount)
        {
            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>()
            {
                new CustomError()
                {
                    ErrorCode = "TX_BALANCE_ERROR",
                    ErrorFriendlyMessage = "The total of the TX or the Debit/credit amounts don't match.",
                    ErrorValueDetails = $"Field:Amount / Value:{amount} - CaluculatedField:DebitAmount / Value:{debitAmount} - CaluculatedField:CreditAmount / Value:{creditAmount}"
                }
            };
        }
    }
}
