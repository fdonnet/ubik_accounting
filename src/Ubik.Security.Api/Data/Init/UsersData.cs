using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Security.Api.Data.Init
{
    internal static class UsersData
    {
        internal static async Task LoadAsync(SecurityDbContext context)
        {
            if (!context.Users.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/UsersData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}
