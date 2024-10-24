using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Security.Api.Data.Init
{
    internal static class RolesAuthorizationsData
    {
        internal static async Task LoadAsync(SecurityDbContext context)
        {
            if (!context.RolesAuthorizations.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/RolesAuthorizationsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}
