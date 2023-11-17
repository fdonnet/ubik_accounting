using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Errors
{
    public record AccountUpdateConcurrencyError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountUpdateConcurrencyError(Guid version)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_UPDATE_CONCURRENCY",
                ErrorFriendlyMessage = "You don't have the last version of this account.",
                ErrorValueDetails = $"Field:Version / Value:{version}"
            }};
        }
    }
}
