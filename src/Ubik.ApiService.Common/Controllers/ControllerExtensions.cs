using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Dto;
using Ubik.ApiService.Common.Exceptions;
using static System.Net.WebRequestMethods;

namespace Ubik.ApiService.Common.Controllers
{
    public static class ControllerExtensions
    {
        public static ActionResult ToOK<TResult, TContract>(this Result<TResult> result, Func<TResult, TContract> mapper
            , IHttpContextAccessor httpContextAccessor)
        {
            return result.Match<ActionResult>(success =>
            {
                var response = mapper(success);
                return new OkObjectResult(response);
            }, exception =>
            {
                return GetObjectResult(exception, httpContextAccessor);
            });
        }

        public static ActionResult ToCreated<TResult, TContract>(this Result<TResult> result, Func<TResult, TContract> mapper
            , string actionName, IHttpContextAccessor httpContextAccessor)
        {
            return result.Match<ActionResult>(success =>
            {
                var response = mapper(success);

                return response is IDtoWithId dto
                    ? new CreatedAtActionResult(actionName, null, new { id = dto.Id }, response)
                    : new StatusCodeResult(500);

            }, exception =>
            {
                return GetObjectResult(exception, httpContextAccessor);
            });
        }

        public static ActionResult ToNoContent<TResult>(this Result<TResult> result, IHttpContextAccessor httpContextAccessor)
        {
            return result.Match<ActionResult>(success =>
            {
                return new NoContentResult();

            }, exception =>
            {
                return GetObjectResult(exception, httpContextAccessor);
            });
        }

        private static ActionResult GetObjectResult(Exception exception, IHttpContextAccessor httpContextAccessor)
        {
            if (exception is ServiceAndFeatureException serviceException && exception != null)
            {
                var serviceProblem = serviceException.ToValidationProblemDetails(httpContextAccessor);

                switch (serviceProblem.Status)
                {
                    case 400:
                        return new BadRequestObjectResult(serviceProblem);
                    case 404:
                        return new NotFoundObjectResult(serviceProblem);
                    case 409:
                        return new ConflictObjectResult(serviceProblem);
                    case 401:
                        return new UnauthorizedObjectResult(serviceProblem);
                    case 403:
                        return new ObjectResult(serviceProblem) { StatusCode = 403 };
                }
            }
            var crashError = new ProblemDetailError() { Code = "UNMANAGED_EXCEPTION", FriendlyMsg = "Contact app support", ValueInError = "" };
            var crashProblemDetails = new CustomProblemDetails(new ProblemDetailError[] { crashError })
            {
                Status = 500,
                Title = "Not tracked exception, bad result",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Instance = httpContextAccessor.HttpContext?.Request.Path,
                Detail = "Not a service exception."
            };
            return new ObjectResult(crashProblemDetails) { StatusCode = 500 };
        }
    }
}
