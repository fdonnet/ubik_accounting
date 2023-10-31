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

    public record CustomError
    {
        public string ErrorFriendlyMessage { get; set; } = default!;
        public string ErrorCode { get; set; }=default!;
        public string? ErrorValueDetails { get; set; }
    }

    public interface IServiceAndFeatureException
    {
        public ServiceAndFeatureExceptionType ErrorType { get; } 
        public List<CustomError> CustomErrors { get; }
    }
}
