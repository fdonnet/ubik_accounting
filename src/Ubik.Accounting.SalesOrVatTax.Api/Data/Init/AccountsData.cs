using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Accounting.SalesOrVatTax.Api.Data.Init
{
    internal static class AccountsData
    {
        internal static async Task LoadAsync(AccountingSalesTaxDbContext context)
        {
            if (!context.Accounts.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/AccountsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}
