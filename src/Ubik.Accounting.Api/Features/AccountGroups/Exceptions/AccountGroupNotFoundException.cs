using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public record AccountGroupNotFoundException : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupNotFoundException(Guid idNotFound)
        {
            ErrorType = ServiceAndFeatureExceptionType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_NOT_FOUND",
                ErrorFriendlyMessage = "The account group doesn't exist. Id not found.",
                ErrorValueDetails = $"Field:Id / Value:{idNotFound}",
            }};
        }
    }
}
