using Ubik.ApiService.Common.Errors;

namespace Ubik.ApiService.Common.Validators
{
    public class CustomValidationException(List<CustomError> errors) : Exception($"Validation errors"), IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; } = FeatureErrorType.BadParams;
        public List<CustomError> CustomErrors { get; init; } = errors;
    }
}
