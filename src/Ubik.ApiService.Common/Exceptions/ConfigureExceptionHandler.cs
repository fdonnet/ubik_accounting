﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
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

                        if (contextFeature.Error is IServiceAndFeatureException managedException)
                        {
                            //Managed excpetion
                            context.Response.StatusCode = (int)managedException.ErrorType;

                            log.LogInformation("Managed exception: type: {type} / Code: {code} / Msg: {msg} / Value: {value} ",
                                managedException.ErrorType, managedException.ErrorCode, managedException.ErrorFriendlyMessage, managedException.ErrorValueDetails);

                            var problemDetails = managedException.ToValidationProblemDetails(context);

                            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails,
                                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }));
                        }
                        else
                        {
                            //Unmanaged exception log as error
                            log.LogError("Something went wrong: {contextFeature.Error}", contextFeature.Error);

                            string? detail = env.IsDevelopment() ? $"{contextFeature.Error}"
                                : "Contact app support !";

                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                            var problemDetail = new CustomProblemDetails(new ProblemDetailError[] {new ProblemDetailError()
                                                        {
                                                            Code = "UNMANAGED_ERROR",
                                                            FriendlyMsg = "Unmanaged exception occurs, see detail field when available.",
                                                            ValueInError = ""
                                                        }})
                            {
                                Status = 500,
                                Title = "Unmanaged error",
                                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                                Detail = detail,
                                Instance = context.Request.Path
                            };

                            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
                            if (traceId != null)
                            {
                                problemDetail.Extensions["traceId"] = traceId;
                            }

                            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetail,
                                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }));

                        }
                    }
                });
            });
        }
    }
}
