using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Users.Commands;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public record UserCannotBeAddedInAuthProviderBadParams : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UserCannotBeAddedInAuthProviderBadParams(AddUserCommand user)
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USER_NOT_ADDED_IN_AUTH_PROVIDER_BAD_PARAM",
                ErrorFriendlyMessage = "The user cannot be added in the auth provider.(check your info, is your email valid ?)",
                ErrorValueDetails = $"Field:Email / Value:{user.Email} | Field:Firstname / Value:{user.Firstname}"
            }};
        }
    }
}
