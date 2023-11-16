using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Classifications.Errors
{
    public class ClassificationIdNotMatchForUpdError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ClassificationIdNotMatchForUpdError(Guid idFromQuery, Guid idFromCommand)
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "CLASSIFICATION_UPDATE_IDS_NOT_MATCH",
                ErrorFriendlyMessage = "The provided id in the query string and in the command don't match.",
                ErrorValueDetails = $"Field:IdQuery|IdCommand / Value:{idFromQuery}|{idFromCommand}"
            }};
        }
    }
}
