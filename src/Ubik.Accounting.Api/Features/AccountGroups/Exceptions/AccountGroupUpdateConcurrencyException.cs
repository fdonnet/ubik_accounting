using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public record AccountGroupUpdateConcurrencyExeception : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupUpdateConcurrencyExeception(Guid version)
        {

            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_UPDATE_CONCURRENCY",
                ErrorFriendlyMessage = "You don't have the last version of this account group.",
                ErrorValueDetails = $"Field:Version / Value:{version}"
            }};
        }
    }
}
