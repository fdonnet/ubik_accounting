using Ubik.ApiService.Common.Errors;

namespace Ubik.Security.Api.Features.Users.Errors
{
    public record UserAlreadyExistsError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public UserAlreadyExistsError(string emailAlreadyExisting)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "USER_ALREADY_EXISTS",
                ErrorFriendlyMessage = "The user already exists. Email field needs to be unique.",
                ErrorValueDetails = $"Field:Email / Value:{emailAlreadyExisting}"
            }};
        }
    }
}
