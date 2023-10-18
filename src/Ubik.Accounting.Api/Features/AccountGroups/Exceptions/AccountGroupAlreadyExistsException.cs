using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public class AccountGroupAlreadyExistsException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupAlreadyExistsException(string codeAlreadyExisting)
         : base($"An account group with this code: {codeAlreadyExisting} already exists.")
        {

            ErrorType = ServiceAndFeatureExceptionType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_ALREADY_EXISTS",
                ErrorFriendlyMessage = "The account group already exists. Code field needs to be unique.",
                ErrorValueDetails = $"Field:Code / Value:{codeAlreadyExisting}"
            }};
        }
    }
}
