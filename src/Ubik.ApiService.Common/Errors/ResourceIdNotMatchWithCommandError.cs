namespace Ubik.ApiService.Common.Errors
{
    public record ResourceIdNotMatchWithCommandError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ResourceIdNotMatchWithCommandError(string resourceName, Guid idFromQuery, Guid idFromCommand)
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"{resourceName.ToUpper()}_COMMAND_IDS_NOT_MATCH",
                ErrorFriendlyMessage = "The provided id in the query string and in the command don't match.",
                ErrorValueDetails = $"Field:IdQuery|IdCommand / Value:{idFromQuery}|{idFromCommand}"
            }];
        }
    }
}
