using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ubik.Security.Api.Features.Tenants.Admin.Services
{
    public class TenantsAdminQueriesService(SecurityDbContext ctx) : ITenantsAdminQueriesService
    {
        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            var result = await ctx.Tenants.ToListAsync();

            return result;
        }

        public async Task<Either<IServiceAndFeatureError, Tenant>> GetAsync(Guid id)
        {
            var result = await ctx.Tenants.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Tenant", "Id", id.ToString())
                : result;
        }
    }
}
