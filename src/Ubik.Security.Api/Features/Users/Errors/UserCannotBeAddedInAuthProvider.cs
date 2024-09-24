using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public class UserCannotBeAddedInAuthProvider : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UserCannotBeAddedInAuthProvider(User user)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USER_NOT_ADDED_IN_AUTH_PROVIDER",
                ErrorFriendlyMessage = "The user cannot be added in the auth provider.",
                ErrorValueDetails = $"""
                                    Field:Email / Value:{user.Email}
                                    Field:Firstname / Value:{user.Firstname}
                                    etc
                                    """
            }};
        }
    }
}
