using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Authorizations.Services
{
    public class AuthorizationsQueriesService(SecurityDbContext ctx) : IAuthorizationsQueriesService
    {
        public async Task<IEnumerable<Authorization>> GetAllAsync()
        {
            var result = await ctx.Authorizations.ToListAsync();

            return result;
        }

        public async Task<Either<IFeatureError, Authorization>> GetAsync(Guid id)
        {
            var result = await ctx.Authorizations.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Authorization", "Id", id.ToString())
                : result;
        }
    }
}
