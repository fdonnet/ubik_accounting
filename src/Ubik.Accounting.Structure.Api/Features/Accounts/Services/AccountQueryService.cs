using Dapper;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Structure.Api.Data;
using Ubik.Accounting.Structure.Api.Features.Accounts.CustomPoco;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Structure.Api.Features.Accounts.Services
{
    public class AccountQueryService(AccountingDbContext ctx, ICurrentUser currentUser) : IAccountQueryService
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

        public async Task<Either<IServiceAndFeatureError, IEnumerable<AccountGroupClassification>>> GetAccountGroupsWithClassificationInfoAsync(Guid id)
        {
            return await GetAsync(id).ToAsync()
                .MapAsync(async ac =>
                {
                    var p = new DynamicParameters();
                    p.Add("@id", id);
                    p.Add("@tenantId", currentUser.TenantId);

                    var con = ctx.Database.GetDbConnection();

                    var sql = """
                              SELECT ag.id
                              , ag.code
                              , ag.label
                              , c.id as classification_id
                              , c.code as classification_code
                              , c.label as classification_label
                              FROM account_groups ag
                              INNER JOIN classifications c ON ag.classification_id = c.id
                              INNER JOIN accounts_account_groups aag ON aag.account_group_id = ag.id
                              WHERE ag.tenant_id = @tenantId
                              AND aag.account_id = @id
                              """;

                    return await con.QueryAsync<AccountGroupClassification>(sql, p);
                });
        }
    }
}
