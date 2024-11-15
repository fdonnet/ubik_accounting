using Ubik.ApiService.Common.Errors;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public record UserTenantNotFound : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UserTenantNotFound(Guid? tenantId)
        {
            var strTenantId = "NULL";
            if(tenantId != null)
                strTenantId = tenantId.ToString();

            ErrorType = FeatureErrorType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USER_SELECTED_TENANT_NOT_FOUND",
                ErrorFriendlyMessage = "The user has no selected tenant or the tenant is not found",
                ErrorValueDetails = $"TenantId = {strTenantId}"
            }};
        }
    }
}
