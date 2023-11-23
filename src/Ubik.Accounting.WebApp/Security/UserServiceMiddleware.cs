using Microsoft.AspNetCore.Authentication;

namespace Ubik.Accounting.WebApp.Security
{
    public class UserServiceMiddleware
    {
        private readonly RequestDelegate next;

        public UserServiceMiddleware(RequestDelegate next)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context, UserService service)
        {
            service.SetUser(context.User);
            await next(context);
        }
    }
}
