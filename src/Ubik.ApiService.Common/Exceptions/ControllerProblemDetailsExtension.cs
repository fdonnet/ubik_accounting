using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Ubik.ApiService.Common.Errors;

namespace Ubik.ApiService.Common.Exceptions
{
    public static class ControllerProblemDetailsExtension
    {
        public static CustomProblemDetails ToValidationProblemDetails(this IFeatureError ex, HttpContext httpContext)
        {
            var problemDetailErrors = ex.CustomErrors.Select(e => new ProblemDetailError()
            {
                ValueInError = e.ErrorValueDetails ?? string.Empty,
                Code = e.ErrorCode,
                FriendlyMsg = e.ErrorFriendlyMessage
            });

            var error = new CustomProblemDetails(problemDetailErrors);

            switch (ex.ErrorType)
            {
                case FeatureErrorType.Conflict:
                    error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
                    error.Status = 409;
                    error.Title = "Resource conflict";
                    break;
                case FeatureErrorType.NotFound:
                    error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                    error.Status = 404;
                    error.Title = "Resource not found";
                    break;
                case FeatureErrorType.NotAuthorized:
                    error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                    error.Status = 403;
                    error.Title = "Resource not authorized";
                    break;
                case FeatureErrorType.NotAuthentified:
                    error.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                    error.Status = 401;
                    error.Title = "No valid authentication detected";
                    break;
                default:
                    error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    error.Status = 400;
                    error.Title = "Bad request or bad params";
                    break;
            }

            error.Detail = "See errors fields for identification and details.";
            error.Instance ??= httpContext.Request.Path;
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            if (traceId != null)
            {
                error.Extensions["traceId"] = traceId;
            }

            return error;
        }
    }
}
