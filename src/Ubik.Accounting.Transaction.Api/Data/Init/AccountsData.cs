using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Accounting.Transaction.Api.Data.Init
{
    internal static class AccountsData
    {
        internal static async Task LoadAsync(AccountingTxContext context)
        {
            if (!context.Accounts.Any())
            {
                var query = await File.ReadAllTextAsync(@"Data/Init/AccountsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(query));
            }
        }
    }
}
