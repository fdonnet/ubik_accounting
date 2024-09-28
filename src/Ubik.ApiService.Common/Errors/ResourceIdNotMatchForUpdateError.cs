namespace Ubik.ApiService.Common.Errors
{
    public record ResourceIdNotMatchForUpdateError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ResourceIdNotMatchForUpdateError(string resourceName, Guid idFromQuery, Guid idFromCommand)
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"{resourceName.ToUpper()}_UPDATE_IDS_NOT_MATCH",
                ErrorFriendlyMessage = "The provided id in the query string and in the command don't match.",
                ErrorValueDetails = $"Field:IdQuery|IdCommand / Value:{idFromQuery}|{idFromCommand}"
            }];
        }
    }
}
