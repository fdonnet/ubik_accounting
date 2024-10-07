using Ubik.ApiService.Common.Errors;

namespace Ubik.Security.Api.Features.RolesAuthorizations.Errors
{
    public record RoleAuthorizationIsNotABaseRoleError : IServiceAndFeatureError
    {

        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public RoleAuthorizationIsNotABaseRoleError(Guid roleId)
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ROLE_AUTHORIZATION_NOT_A_BASE_ROLE_OR_NOT_EXISTS",
                ErrorFriendlyMessage = "Role not exists or cannot asign an authorization to a tenant role with the admin function.",
                ErrorValueDetails = $"Field:RoleId / Value:{roleId}"
            }};
        }
    }

}
