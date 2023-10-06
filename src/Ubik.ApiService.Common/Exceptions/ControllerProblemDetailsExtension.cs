using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Exceptions
{
    //TODO: see if it's anti pattern to use this extension.... method on ServiceException with httpContectAccessor = I think yes
    public static class ControllerProblemDetailsExtension
    {
        public static CustomProblemDetails ToValidationProblemDetails(this ServiceException ex, IHttpContextAccessor httpContextAccessor)
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
                error.Instance ??= httpContextAccessor.HttpContext?.Request.Path;

                var traceId = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
                if (traceId != null)
                {
                    error.Extensions["traceId"] = traceId;
                }
            }

            return error;
        }
    }
}
