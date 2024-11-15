using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Accounting.Structure.Api.Data.Init
{
    internal static class AccountGroupsData
    {
        internal static async Task LoadAsync(AccountingDbContext context)
        {
            if (!context.AccountGroups.Any())
            {
                var accountGrpsQuery = await File.ReadAllTextAsync(@"Data/Init/AccountGroupsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(accountGrpsQuery));
            }
        }
    }
}
