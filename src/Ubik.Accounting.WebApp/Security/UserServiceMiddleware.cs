namespace Ubik.Accounting.WebApp.Security
{
    public class UserServiceMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));

        public async Task InvokeAsync(HttpContext context, UserService service)
        {
            service.SetUser(context.User);
            await next(context);
        }
    }
}
