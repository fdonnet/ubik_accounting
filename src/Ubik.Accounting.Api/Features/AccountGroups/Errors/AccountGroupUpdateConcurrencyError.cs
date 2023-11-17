using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Errors
{
    public record AccountGroupUpdateConcurrencyError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupUpdateConcurrencyError(Guid version)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_UPDATE_CONCURRENCY",
                ErrorFriendlyMessage = "You don't have the last version of this account group.",
                ErrorValueDetails = $"Field:Version / Value:{version}"
            }};
        }
    }
}
