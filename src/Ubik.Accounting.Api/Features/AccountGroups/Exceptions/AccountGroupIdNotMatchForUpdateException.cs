using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Exceptions
{
    public class AccountGroupIdNotMatchForUpdateException : IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public AccountGroupIdNotMatchForUpdateException(Guid idFromQuery, Guid idFromCommand)
        {

            ErrorType = ServiceAndFeatureExceptionType.BadParams;
            CustomErrors = new List<CustomError>() { new CustomError()
            {
                ErrorCode = "ACCOUNTGROUP_UPDATE_IDS_NOT_MATCH",
                ErrorFriendlyMessage = "The provided id in the query string and in the command don't match.",
                ErrorValueDetails = $"Field:IdQuery|IdCommand / Value:{idFromQuery}|{idFromCommand}"
            }};
        }
    }
}
