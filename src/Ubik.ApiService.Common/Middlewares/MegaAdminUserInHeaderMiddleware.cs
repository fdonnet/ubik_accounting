using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;

namespace Ubik.ApiService.Common.Middlewares
{
    internal static class UserMiddlewareErrors
    {
        public static async Task Manage(HttpContext context, string errorMsg)       {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problemDetail = new CustomProblemDetails(new ProblemDetailError[] {new()
                                                        {
                                                            Code = "HEADER_VALUE_MISSING",
                                                            FriendlyMsg = errorMsg,
                                                            ValueInError = ""
                                                        }})
            {
                Status = 400,
                Title = "Missing param in header",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Instance = context.Request.Path
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetail,
                                    serializeOptions));
        }

        private static readonly JsonSerializerOptions serializeOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public class MegaAdminUserInHeaderMiddleware(RequestDelegate next)
    {
        //TODO: megaadmin hardcoded here, really bad. MegaAdmin doesn't need a tenant_id
        private static readonly Guid MegaAminUserId = new("5c5e0000-3c36-7456-b9da-08dcdf9832e2");

        public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
        {
            //UserId
            if (context.Request.Headers.TryGetValue("x-user-id", out var userId))
            {
                if (Guid.TryParse(userId, out var parsedUserId))
                {
                    if(parsedUserId == MegaAminUserId )
                        currentUser.Id = parsedUserId;
                    else
                    {
                        await UserMiddlewareErrors.Manage(context, "Invalid x-user-id for MegaAdmin");
                        return;
                    }
                }
                else
                {
                    await UserMiddlewareErrors.Manage(context, "Invalid x-user-id format");
                    return;
                }
            }
            else
            {
                await UserMiddlewareErrors.Manage(context, "x-user-id is missing");
                return;
            }

            await next(context);
        }
    }
    public class UserInHeaderMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
        {
            //UserId
            if (context.Request.Headers.TryGetValue("x-user-id", out var userId))
            {
                if (Guid.TryParse(userId, out var parsedUserId))
                {
                    currentUser.Id = parsedUserId;
                }
                else
                {
                    await UserMiddlewareErrors.Manage(context, "Invalid x-user-id format");
                    return;
                }
            }
            else
            {
                await UserMiddlewareErrors.Manage(context, "x-user-id is missing");
                return;
            }

            //TenantId
            if (context.Request.Headers.TryGetValue("x-tenant-id", out var tenantId))
            {
                if (Guid.TryParse(tenantId, out var parsedTenantId))
                {
                    currentUser.TenantId = parsedTenantId;
                }
                else
                {
                    await UserMiddlewareErrors.Manage(context, "Invalid x-tenant-id format");
                    return;
                }
            }
            else
            {
                await UserMiddlewareErrors.Manage(context, "TenantId not specified");
                return;
            }

            await next(context);
        }
    }
}
