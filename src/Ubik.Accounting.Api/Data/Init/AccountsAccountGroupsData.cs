using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Init
{
    internal class AccountsAccountGroupsData
    {
        internal static async Task LoadAsync(AccountingContext context)
        {
            if (!context.AccountsAccountGroups.Any())
            {
                var accountsAccountGrpsQuery = await File.ReadAllTextAsync(@"Data\Init\AccountsAccountGroupsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(accountsAccountGrpsQuery));
            }
        }
    }
}
