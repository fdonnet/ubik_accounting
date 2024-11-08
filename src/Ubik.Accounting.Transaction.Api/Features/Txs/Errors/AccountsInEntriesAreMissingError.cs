using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Errors
{
    //TODO: do better error reporting (see when UI)
    public record AccountsInEntriesAreMissingError: IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountsInEntriesAreMissingError(IEnumerable<SubmitTxEntry> entriesInError)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>();

            foreach (var entry in entriesInError)
            {
                CustomErrors.Add(new CustomError()
                {
                    ErrorCode = "ACCOUNT_ID_NOT_FOUND_FOR_THIS_ENTRY",
                    ErrorFriendlyMessage = "This account id is not found and cannot be used.",
                    ErrorValueDetails = $"Field:AccountId / Value:{entry.AccountId} - Field:Amount / Value:{entry.Amount}"
                });
            }
        }
    }
}
