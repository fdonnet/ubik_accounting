using Ubik.ApiService.Common.Errors;

namespace Ubik.ApiService.Common.Validators
{
    public class CustomValidationException(List<CustomError> errors) : Exception($"Validation errors"), IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; } = ServiceAndFeatureErrorType.BadParams;
        public List<CustomError> CustomErrors { get; init; } = errors;
    }
}
