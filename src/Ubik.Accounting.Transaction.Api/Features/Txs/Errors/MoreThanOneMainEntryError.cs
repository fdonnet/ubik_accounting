using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Errors
{
    public class MoreThanOneMainEntryError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public MoreThanOneMainEntryError()
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>()
            {
                new CustomError()
                {
                    ErrorCode = "TX_COUNTAINS_MORE_THAN_ONE_MAIN_ENTRY",
                    ErrorFriendlyMessage = "A tx cannot contain more than one entry",
                    ErrorValueDetails = string.Empty,
                }
            };
        }
    }
}

