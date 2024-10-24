using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Roles.Services
{
    public class RolesAdminQueriesService(SecurityDbContext ctx) : IRolesAdminQueriesService
    {
        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            var result = await ctx.Roles.Where(r => r.TenantId == null).ToListAsync();

            return result;
        }

        public async Task<Either<IServiceAndFeatureError, Role>> GetAsync(Guid id)
        {
            var result = await ctx.Roles.FirstOrDefaultAsync(r => r.Id == id && r.TenantId == null);

            return result == null
                ? new ResourceNotFoundError("Role", "Id", id.ToString())
                : result;
        }
    }
}
