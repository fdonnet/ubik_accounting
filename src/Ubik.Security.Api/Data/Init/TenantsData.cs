using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Security.Api.Data.Init
{
    internal static class TenantsData
    {
        internal static async Task LoadAsync(SecurityDbContext context)
        {
            if (!context.Tenants.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/TenantsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}
