using Ubik.ApiService.Common.Exceptions;

namespace Ubik.ApiService.Common.Validators
{
    public class CustomValidationException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public CustomValidationException(List<CustomError> errors)
         : base($"Validation errors")
        {
            ErrorType = ServiceAndFeatureExceptionType.BadParams;
            CustomErrors = errors;
        }
    }
}
