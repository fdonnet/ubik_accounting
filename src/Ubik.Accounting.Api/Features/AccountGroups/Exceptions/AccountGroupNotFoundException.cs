using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public class AccountGroupNotFoundException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupNotFoundException(Guid idNotFound)
         : base($"An account group with this id: {idNotFound} is not found.")
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
