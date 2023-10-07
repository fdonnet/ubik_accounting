using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;

namespace Ubik.ApiService.Common.Exceptions
{
    public class CustomProblemDetailsFactory : ProblemDetailsFactory
    {
        public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string? title = null,
            string? type = null, string? detail = null, string? instance = null)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            return problemDetails;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext,
            ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null, string? type = null,
            string? detail = null, string? instance = null)
        {
            statusCode ??= 400;
            type ??= "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            instance ??= httpContext.Request.Path;
            detail ??= "See errors fields for identification and details.";

            var problemDetails = new CustomProblemDetails(modelStateDictionary)
            {
                Status = statusCode,
                Type = type,
                Instance = instance,
                Detail = detail
            };

            if (title != null)
            {
                // For validation problem details, don't overwrite the default title with null.
                problemDetails.Title = title;
            }

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null)
            {
                problemDetails.Extensions["traceId"] = traceId;
            }

            return problemDetails;
        }

        public CustomProblemDetails CreateUnmanagedProblemDetails(HttpContext httpContext,
            ProblemDetailErrors[] errors, int? statusCode = null, string? title = null, string? type = null,
            string? detail = null, string? instance = null)
        {
            statusCode ??= 500;
            type ??= "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            instance ??= httpContext.Request.Path;
            detail ??= "Contact the app support !";

            var problemDetails = new CustomProblemDetails(errors)
            {
                Status = statusCode,
                Type = type,
                Instance = instance,
                Detail = detail
            };

            if (title != null)
            {
                // For validation problem details, don't overwrite the default title with null.
                problemDetails.Title = title;
            }

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null)
            {
                problemDetails.Extensions["traceId"] = traceId;
            }

            return problemDetails;
        }
    }

}
