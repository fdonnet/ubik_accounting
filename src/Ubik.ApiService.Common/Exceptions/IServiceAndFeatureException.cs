namespace Ubik.ApiService.Common.Exceptions
{
    public enum ServiceAndFeatureExceptionType
    {
        NotFound = 404,
        BadParams = 400,
        Conflict = 409,
        NotAuthorized = 403,
        NotAuthentified = 401
    }

    //TODO: TO BE REMOVED, only keep the interface
    public class ServiceAndFeatureException : Exception, IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; } //Allow to identify what we need to do
        public string ErrorFriendlyMessage { get; init; } = default!; //English message for internal purposes
        public string ErrorCode { get; init; } = default!; //Error code that need to be included to precily idendify the error an maybe manage translation etc
        public string ErrorValueDetails { get; init; } = default!; //The value that raises the error
    }

    public interface IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; } //Allow to identify what we need to do
        public string ErrorFriendlyMessage { get; init; } //English message for internal purposes
        public string ErrorCode { get; init; } //Error code that need to be included to precily idendify the error an maybe manage translation etc
        public string ErrorValueDetails { get; init; } //The value that raises the error
    }
}
