using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Security.Api.Data.Init
{
    internal static class RolesData
    {
        internal static async Task LoadAsync(SecurityDbContext context)
        {
            if (!context.Authorizations.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/RolesData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}
