using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public class AccountGroupQueryService(AccountingDbContext ctx) : IAccountGroupQueryService
    {
        public async Task<IEnumerable<AccountGroup>> GetAllAsync()
        {
            return await ctx.AccountGroups.ToListAsync();
        }

        public async Task<Either<IServiceAndFeatureError, AccountGroup>> GetAsync(Guid id)
        {
            var accountGroup = await ctx.AccountGroups.FindAsync(id);

            return accountGroup == null
                ? new ResourceNotFoundError("AccountGroup", "Id", id.ToString())
                : accountGroup;
        }
    }
}
