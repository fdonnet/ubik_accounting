using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Enums;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class AccountsData
    {
        internal static async Task LoadAsync(AccountingContext context)
        {
            if (!context.Accounts.Any())
            {
                var accountsQuery = await File.ReadAllTextAsync(@"Data/Init/AccountsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(accountsQuery));
            }
        }
    }
}
