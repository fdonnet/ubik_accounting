using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Errors
{
    public record AccountGroupAlreadyExistsError : IServiceAndFeatureError
    {
        public ServiceAndFeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupAlreadyExistsError(string codeAlreadyExisting, Guid accountGroupClassification)
        {

            ErrorType = ServiceAndFeatureErrorType.Conflict;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_ALREADY_EXISTS",
                ErrorFriendlyMessage = "The account group already exists. Code field needs to be unique in a  group classification.",
                ErrorValueDetails = $"Field:Code|AccountGroupClassificationId / Value:{codeAlreadyExisting}|{accountGroupClassification}"
            }};
        }
    }
}
