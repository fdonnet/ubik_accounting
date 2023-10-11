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

    public struct CustomError
    {
        public string ErrorFriendlyMessage;
        public string ErrorCode;
        public string ErrorValueDetails;
    }

    public interface IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; init; } //Allow to identify what we need to do
        public List<CustomError> CustomErrors { get; init; }
    }
}
