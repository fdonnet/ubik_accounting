using Ubik.ApiService.Common.Errors;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public record UserCannotGetMainUsrMgtRole : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UserCannotGetMainUsrMgtRole()
        {
            ErrorType = ServiceAndFeatureErrorType.ServerError;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "CANNOT_RETRIEVE_NECESSARY_ROLE",
                ErrorFriendlyMessage = "System error, contact the system administrator.",
                ErrorValueDetails = $"No info"
            }};
        }
    }
}
