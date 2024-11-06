using Dapper;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Structure.Api.Data;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Structure.Api.Features.AccountGroups.Services
{
    public class AccountGroupQueryService(AccountingDbContext ctx, ICurrentUser currentUser) : IAccountGroupQueryService
    {
        public async Task<IEnumerable<AccountGroup>> GetAllAsync()
        {
            return await ctx.AccountGroups.ToListAsync();
        }

        public async Task<Either<IFeatureError, AccountGroup>> GetAsync(Guid id)
        {
            var accountGroup = await ctx.AccountGroups.FindAsync(id);

            return accountGroup == null
                ? new ResourceNotFoundError("AccountGroup", "Id", id.ToString())
                : accountGroup;
        }

        public async Task<Either<IFeatureError, IEnumerable<Account>>> GetChildAccountsAsync(Guid id)
        {
            return await GetAsync(id)
                .MapAsync(async a =>
                {
                    var p = new DynamicParameters();
                    p.Add("@account_group_id", id);
                    p.Add("@tenantId", currentUser.TenantId);

                    var con = ctx.Database.GetDbConnection();
                    var sql = """
                              SELECT a.* 
                              FROM accounts a
                              INNER JOIN accounts_account_groups aag ON a.id = aag.account_id
                              WHERE aag.account_group_id = @account_group_id
                              """;

                    return await con.QueryAsync<Account>(sql, p);
                });
        }
    }
}
