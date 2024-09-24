using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public class CannotGetAuthToken : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public CannotGetAuthToken()
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USER_CANNOT_GET_AUTH_TOKEN",
                ErrorFriendlyMessage = "Cannot retrieve valid token to communicate with the auth provider.",
                ErrorValueDetails = $"Token is null."
            }};
        }
    }
}
