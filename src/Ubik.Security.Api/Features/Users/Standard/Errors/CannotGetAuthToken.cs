using Ubik.ApiService.Common.Errors;

namespace Ubik.Security.Api.Features.Users.Standard.Errors
{
    public record CannotGetAuthToken : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public CannotGetAuthToken()
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USER_CANNOT_GET_AUTH_TOKEN",
                ErrorFriendlyMessage = "Cannot retrieve valid token to communicate with the auth provider.",
                ErrorValueDetails = $"Token is null."
            }};
        }
    }
}
