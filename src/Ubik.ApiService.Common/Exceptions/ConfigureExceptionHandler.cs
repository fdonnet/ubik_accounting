using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Exceptions
{
    public static class ConfigureExceptionHandler
    {
        //This exception handler is only used for unmanaged exceptions or exceptions that has been thrown.
        //Other exception (managed) are not thrown, we use a TResult patern to trap them (better perf)
        //This will only return 500 status code (unmanaged here)
        public static void UseExceptionHandler(this IApplicationBuilder app, ILogger log, IHostEnvironment env, ProblemDetailsFactory problemDetailsFactory)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        context.Response.ContentType = "application/json";

                        //Unmanaged exception log as error
                        log.LogError("Something went wrong: {contextFeature.Error}", contextFeature.Error);

                        string? detail = env.IsDevelopment() ? $"{contextFeature.Error}"
                            : null;

                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;


                        if(problemDetailsFactory is CustomProblemDetailsFactory customProblemDetailsFactory)
                        {
                            var error = new ProblemDetailErrors() { Code = "UNMANAGED_ERROR", 
                                                                    FriendlyMsg = "Unmanaged exception occurs, see detail field when available.",
                                                                    ValueInError = "" };

                            var problemDetail = customProblemDetailsFactory.CreateUnmanagedProblemDetails(context,
                                                                                        new ProblemDetailErrors[] {error},
                                                                                        500,
                                                                                        "Unmanaged error",
                                                                                        "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                                                                                        detail,
                                                                                        context.Request.Path);

                            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetail,
                                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}));
                        }
                        else
                        {
                            var problemDetail = problemDetailsFactory.CreateProblemDetails(context,
                                                                                        500,
                                                                                        "Unmanaged error",
                                                                                        "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                                                                                        detail,
                                                                                        context.Request.Path);

                            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetail,
                                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }));
                        }
                    }
                });
            });
        }

    }
}
