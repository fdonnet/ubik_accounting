﻿using Ubik.ApiService.Common.Errors;
using Ubik.Security.Contracts.Users.Commands;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public record UserCannotBeAddedInAuthProviderConflict : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UserCannotBeAddedInAuthProviderConflict(AddUserCommand user)
        {

            ErrorType = FeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USER_NOT_ADDED_IN_AUTH_PROVIDER_CONFLICT",
                ErrorFriendlyMessage = "The user cannot be added in the auth provider.(check your info). Email/user is already existing",
                ErrorValueDetails = $"Field:Email Value:{user.Email}"
            }};
        }
    }
}
