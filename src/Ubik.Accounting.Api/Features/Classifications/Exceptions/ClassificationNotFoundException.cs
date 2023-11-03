using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Classifications.Exceptions
{
    public record ClassificationNotFoundException : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ClassificationNotFoundException(Guid id)
        {

            ErrorType = ServiceAndFeatureExceptionType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "CLASSIFICATION_NOT_FOUND",
                ErrorFriendlyMessage = "The classification is not found.",
                ErrorValueDetails = $"Field:Id / Value:{id}"
            }};
        }
    }
}
