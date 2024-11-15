namespace Ubik.ApiService.Common.Errors
{
    public record ResourceNotFoundError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ResourceNotFoundError(string resourceName, string fieldName, string value)
        {
            ErrorType = FeatureErrorType.NotFound;
            CustomErrors = [ new CustomError()
            {
                ErrorCode = $"{resourceName.ToUpper()}_NOT_FOUND",
                ErrorFriendlyMessage = $"The {resourceName} doesn't exist. {fieldName} not found.",
                ErrorValueDetails = $"Field:{fieldName} / Value:{value}",
            }];
        }
    }
}
