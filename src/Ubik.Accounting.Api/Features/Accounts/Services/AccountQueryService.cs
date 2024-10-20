using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public class AccountQueryService(AccountingDbContext ctx) : IAccountQueryService
    {
        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await ctx.Accounts.ToListAsync();
        }

        public async Task<Either<IServiceAndFeatureError, Account>> GetAsync(Guid id)
        {
            var account = await ctx.Accounts.FindAsync(id);

            return account == null
                ? new ResourceNotFoundError("Account", "Id", id.ToString())
                : account;
        }

        public async Task<IEnumerable<AccountAccountGroup>> GetAllAccountGroupLinksAsync()
        {
            var results = await ctx.AccountsAccountGroups.ToListAsync();

            return results;
        }

    }
}
