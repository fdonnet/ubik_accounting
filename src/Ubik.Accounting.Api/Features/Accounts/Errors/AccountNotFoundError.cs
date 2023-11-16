using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Errors
{
    public record AccountNotFoundError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountNotFoundError(Guid idNotFound)
        {
            ErrorType = ServiceAndFeatureErrorType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNT_NOT_FOUND",
                ErrorFriendlyMessage = "The account doesn't exist. Id not found.",
                ErrorValueDetails = $"Field:Id / Value:{idNotFound}",
            }};
        }
    }
}
