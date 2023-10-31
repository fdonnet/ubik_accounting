using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Exceptions
{
    public record AccountUpdateConcurrencyExeception : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountUpdateConcurrencyExeception(Guid version)
        {

            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_UPDATE_CONCURRENCY",
                ErrorFriendlyMessage = "You don't have the last version of this account.",
                ErrorValueDetails = $"Field:Version / Value:{version}"
            }};
        }
    }
}
