using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public class AccountGroupParentNotFoundException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupParentNotFoundException(Guid parentAccountGroupId)
         : base($"An parent account group with this id: {parentAccountGroupId} is not found.")
        {

            ErrorType = ServiceAndFeatureExceptionType.NotFound;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "PARENT_ACCOUNTGROUP_NOTFOUND",
                ErrorFriendlyMessage = "The parent account group is not found.",
                ErrorValueDetails = $"Field:ParentAccountGroupId / Value:{parentAccountGroupId}"
            }};
        }
    }
}
