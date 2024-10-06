using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Authorizations.Admin.Services
{
    public class AuthorizationsAdminQueriesService(SecurityDbContext ctx) : IAuthorizationsAdminQueriesService
    {
        public async Task<IEnumerable<Authorization>> GetAllAsync()
        {
            var result = await ctx.Authorizations.ToListAsync();

            return result;
        }

        public async Task<Either<IServiceAndFeatureError, Authorization>> GetAsync(Guid id)
        {
            var result = await ctx.Authorizations.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Auhtorization", "Id", id.ToString())
                : result;
        }
    }
}
