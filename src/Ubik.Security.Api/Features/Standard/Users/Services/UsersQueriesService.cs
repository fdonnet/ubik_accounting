using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Standard.Users.Services
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
    }
}
