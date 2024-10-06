using Ubik.ApiService.Common.Errors;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public record UserAddFatalError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UserAddFatalError()
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USER_NOT_ADDED_IN_DB_BUT_NOT_IN_AUTH",
                ErrorFriendlyMessage = "Fatal error, contact your admin, you will not be able to add this new user",
                ErrorValueDetails = $""
            }};
        }
    }
}
