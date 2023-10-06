using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ubik.ApiService.Common.Dto;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.ApiService.Common.Controllers
{
    public static class ControllerExtensions
    {
        public static ActionResult ToOK<TResult,TContract>(this Result<TResult> result, Func<TResult,TContract> mapper, HttpContext httpContext)
        {
            return result.Match<ActionResult>(success =>
            {
                var response = mapper(success);
                return new OkObjectResult(response);
            }, exception =>
            {
                if (exception is ServiceException serviceException && exception != null)
                {
                    var serviceProblem = serviceException.ToValidationProblemDetails(httpContext);
                    return new ConflictObjectResult(serviceProblem);
                }
                return new StatusCodeResult(500);
            });
        }

        public static ActionResult ToCreated<TResult, TContract>(this Result<TResult> result, Func<TResult, TContract> mapper, string actionName, HttpContext httpContext)
        {
            return result.Match<ActionResult>(success =>
            {
                var response = mapper(success);

                return response is IDtoWithId dto
                    ? new CreatedAtActionResult(actionName, null, new { id = dto.Id }, response)
                    : new StatusCodeResult(500);

            }, exception =>
            {
                if (exception is ServiceException serviceException && exception != null)
                {
                    var serviceProblem = serviceException.ToValidationProblemDetails(httpContext);
                    return new ConflictObjectResult(serviceProblem);
                }
                return new StatusCodeResult(500);
            });
        }
    }
}
