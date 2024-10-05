using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Users.Services
{
    public class UsersQueriesService(SecurityDbContext ctx) : IUsersQueriesService
    {
        public async Task<Either<IServiceAndFeatureError, User>> GetAsync(Guid id)
        {
            var result = await ctx.Users.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("User", "Id", id.ToString())
                : result;
        }

        public async Task<Either<IServiceAndFeatureError, User>> GetAsync(string email)
        {
            var result = await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);

            return result == null
                ? new ResourceNotFoundError("User", "Email", email.ToString())
                : result;
        }
    }
}
