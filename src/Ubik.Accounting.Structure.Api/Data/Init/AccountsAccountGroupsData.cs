using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ubik.Accounting.Structure.Api.Data.Init
{
    internal class AccountsAccountGroupsData
    {
        internal static async Task LoadAsync(AccountingDbContext context)
        {
            if (!context.AccountsAccountGroups.Any())
            {
                var accountsAccountGrpsQuery = await File.ReadAllTextAsync(@"Data/Init/AccountsAccountGroupsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(accountsAccountGrpsQuery));
            }
        }
    }
}
