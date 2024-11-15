using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Errors
{
    //TODO: do better error reporting (see when UI)
    public record AccountAreNotFoundErrors: IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountAreNotFoundErrors(IEnumerable<SubmitTxEntry> entriesInError)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>();

            foreach (var entry in entriesInError)
            {
                CustomErrors.Add(new CustomError()
                {
                    ErrorCode = "ACCOUNT_NOT_FOUND_FOR_ENTRY",
                    ErrorFriendlyMessage = "This account is not found.",
                    ErrorValueDetails = $"Field:AccountId / Value:{entry.AccountId}"
                });
            }
        }
    }
}
