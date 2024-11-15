namespace Ubik.ApiService.Common.Errors
{
    public record BadParamExternalResourceNotFound : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public BadParamExternalResourceNotFound(string resourceName, string externalResourceName, string fieldName, string value)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new CustomError()
            {
                ErrorCode = $"{resourceName.ToUpper()}_{externalResourceName.ToUpper()}_NOT_FOUND",
                ErrorFriendlyMessage = $"The {externalResourceName} doesn't exist. Param {fieldName} not found.",
                ErrorValueDetails = $"Field:{fieldName} / Value:{value}",
            }];
        }
    }
}
