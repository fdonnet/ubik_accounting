using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.AccountGroups.Errors
{
    public record AccountGroupParentNotFoundError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupParentNotFoundError(Guid parentAccountGroupId)
        {

            ErrorType = ServiceAndFeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "PARENT_ACCOUNTGROUP_NOTFOUND",
                ErrorFriendlyMessage = "The parent account group specified is not found.",
                ErrorValueDetails = $"Field:ParentAccountGroupId / Value:{parentAccountGroupId}"
            }};
        }
    }
}
