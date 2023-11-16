using Ubik.ApiService.Common.Errors;

namespace Ubik.ApiService.Common.Exceptions
{
    public class UpdateDbConcurrencyException : Exception, IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UpdateDbConcurrencyException()
        {
            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "DB_CONCURRENCY_CONFLICT",
                ErrorFriendlyMessage = "You don't have the last version of this ressource, refresh your data before updating.",
                ErrorValueDetails = $"Field:Version",
            }};
        }
    }
}
