﻿using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Ubik.ApiService.Common.Exceptions
{
    public static class ControllerProblemDetailsExtension
    {
        public static CustomProblemDetails ToValidationProblemDetails(this IServiceAndFeatureException ex, HttpContext httpContext)
        {
            var error = new CustomProblemDetails(new List<ProblemDetailError>()
                                                    {
                                                        new ProblemDetailError()
                                                        {
                                                            Code =ex.ErrorCode,
                                                            FriendlyMsg = ex.ErrorFriendlyMessage,
                                                            ValueInError = ex.ErrorValueDetails
                                                        }});

            switch (ex.ErrorType)
            {
                case ServiceAndFeatureExceptionType.Conflict:
                    error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
                    error.Status = 409;
                    error.Title = "Resource conflict (already exists)";
                    break;
                case ServiceAndFeatureExceptionType.NotFound:
                    error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                    error.Status = 404;
                    error.Title = "Resource not found";
                    break;
                case ServiceAndFeatureExceptionType.NotAuthorized:
                    error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                    error.Status = 403;
                    error.Title = "Resource not authorized";
                    break;
                case ServiceAndFeatureExceptionType.NotAuthentified:
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
