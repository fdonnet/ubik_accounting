using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public record AccountGroupClassificationNotFound : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupClassificationNotFound(Guid classificationId)
        {

            ErrorType = ServiceAndFeatureExceptionType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_CLASSIFICATION_NOTFOUND",
                ErrorFriendlyMessage = "The account group classification specified is not found.",
                ErrorValueDetails = $"Field:AccountGroupClassificationId / Value:{classificationId}"
            }};
        }
    }
}
