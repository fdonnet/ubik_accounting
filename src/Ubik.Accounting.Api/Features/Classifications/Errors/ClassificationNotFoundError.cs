using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Classifications.Errors
{
    public record ClassificationNotFoundError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ClassificationNotFoundError(Guid id)
        {

            ErrorType = ServiceAndFeatureErrorType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "CLASSIFICATION_NOT_FOUND",
                ErrorFriendlyMessage = "The classification is not found.",
                ErrorValueDetails = $"Field:Id / Value:{id}"
            }};
        }
    }
}
