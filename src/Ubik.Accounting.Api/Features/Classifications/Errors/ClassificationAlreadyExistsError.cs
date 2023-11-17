using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Classifications.Errors
{
    public class ClassificationAlreadyExistsError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ClassificationAlreadyExistsError(string code)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "CLASSIFICATION_ALREADY_EXISTS",
                ErrorFriendlyMessage = "The classification already exists. Code field needs to be unique.",
                ErrorValueDetails = $"Field:Code / Value:{code}"
            }};
        }
    }
}
