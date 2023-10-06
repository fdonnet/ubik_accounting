using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ubik.ApiService.Common.Exceptions
{
    public enum ServiceExceptionType
    {
        OK = 1,
        NotFound = 2,
        BadParams = 3,
        AlreadyExists = 4,
        NotAuthorized = 5,
        NotAuthentified = 6,
    }
    public class ServiceException : Exception
    {
        public ServiceExceptionType ExceptionType { get; set; } //Allow to identify what we need to do
        public required string ErrorFriendlyMessage { get; set; } //English message for internal purposes
        public required string ErrorCode { get; set; } //Error code that need to be included to precily idendify the error an maybe manage translation etc
        public required string ErrorValueDetails { get; set; } //The value that raises the error
    }

    public static class ServiceExceptionProblemDetails
    {
        public static CustomProblemDetails ToValidationProblemDetails(this ServiceException ex, HttpContext httpContext)
        {
            var error = new CustomProblemDetails(new List<ProblemDetailErrors>()
                                                    {
                                                        new ProblemDetailErrors()
                                                        {
                                                            Code =ex.ErrorCode,
                                                            FriendlyMsg = ex.ErrorFriendlyMessage,
                                                            ValueInError = ex.ErrorValueDetails
                                                        }});

            if (ex.ExceptionType == ServiceExceptionType.AlreadyExists)
            {
                error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
                error.Status = 409;
                error.Title = "Ressource already exists";
                error.Detail = "See errors fields for identification and details.";
                error.Instance ??= httpContext.Request.Path;

                var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
                if (traceId != null)
                {
                    error.Extensions["traceId"] = traceId;
                }
            }

            return error;
        }
    }



}
