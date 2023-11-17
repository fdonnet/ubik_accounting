using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Errors
{
    public record AccountGroupClassificationNotFoundError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupClassificationNotFoundError(Guid classificationId)
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_CLASSIFICATION_NOTFOUND",
                ErrorFriendlyMessage = "The account group classification specified is not found.",
                ErrorValueDetails = $"Field:AccountGroupClassificationId / Value:{classificationId}"
            }};
        }
    }
}
