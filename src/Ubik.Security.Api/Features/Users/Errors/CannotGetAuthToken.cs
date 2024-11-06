using Ubik.ApiService.Common.Errors;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public record CannotGetAuthToken : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public CannotGetAuthToken()
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USER_CANNOT_GET_AUTH_TOKEN",
                ErrorFriendlyMessage = "Cannot retrieve valid token to communicate with the auth provider.",
                ErrorValueDetails = $"Token is null."
            }};
        }
    }
}
