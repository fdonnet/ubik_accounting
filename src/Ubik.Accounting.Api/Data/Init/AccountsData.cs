using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class AccountsData
    {
        internal static async Task LoadAsync(AccountingDbContext context)
        {
            if (!context.Accounts.Any())
            {
                var accountsQuery = await File.ReadAllTextAsync(@"Data/Init/AccountsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(accountsQuery));
            }
        }
    }
}
