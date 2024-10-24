using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Security.Api.Data.Init
{
    internal static class UserRoleByTenantData
    {
        internal static async Task LoadAsync(SecurityDbContext context)
        {
            if (!context.UserRolesByTenants.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/UserRoleByTenantData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}
