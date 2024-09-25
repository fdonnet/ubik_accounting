using Ubik.ApiService.Common.Errors;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public record UserNotFoundError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UserNotFoundError(Guid idNotFound)
        {
            ErrorType = ServiceAndFeatureErrorType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USER_NOT_FOUND",
                ErrorFriendlyMessage = "The account doesn't exist. Id not found.",
                ErrorValueDetails = $"Field:Id / Value:{idNotFound}",
            }};
        }
    }

}
