using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class AccountGroupsData
    {
        internal static async Task LoadAsync(AccountingContext context)
        {
            if (!context.AccountGroups.Any())
            {
                var accountGrpsQuery = await File.ReadAllTextAsync(@"Data/Init/AccountGroupsData.sql");
                await context.Database.ExecuteSqlAsync(FormattableStringFactory.Create(accountGrpsQuery));
            }
        }
    }
}
