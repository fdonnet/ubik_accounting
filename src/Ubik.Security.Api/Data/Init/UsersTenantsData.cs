using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Security.Api.Data.Init
{
    internal static class UsersTenantsData
    {
        internal static async Task LoadAsync(SecurityDbContext context)
        {
            if (!context.UsersTenants.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/UsersTenantsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}
