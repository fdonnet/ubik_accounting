using Ubik.ApiService.Common.Errors;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public record UserCannotCheckIfPresentInAuth : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UserCannotCheckIfPresentInAuth()
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USERs_CANNOT_CHECK_IF_PRESENT_IN_AUTH",
                ErrorFriendlyMessage = "The auth provider cannot be called and checked for users.",
                ErrorValueDetails = $""
            }};
        }
    }
}
