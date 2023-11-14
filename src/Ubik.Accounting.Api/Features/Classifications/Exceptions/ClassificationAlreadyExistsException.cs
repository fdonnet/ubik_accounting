using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Classifications.Exceptions
{
    public class ClassificationAlreadyExistsException : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ClassificationAlreadyExistsException(string code)
        {

            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "CLASSIFICATION_ALREADY_EXISTS",
                ErrorFriendlyMessage = "The classification already exists. Code field needs to be unique.",
                ErrorValueDetails = $"Field:Code / Value:{code}"
            }};
        }
    }
}
