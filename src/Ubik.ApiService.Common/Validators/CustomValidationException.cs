using Ubik.ApiService.Common.Errors;

namespace Ubik.ApiService.Common.Validators
{
    public class CustomValidationException : Exception, IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public CustomValidationException(List<CustomError> errors)
         : base($"Validation errors")
        {
            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = errors;
        }
    }
}
