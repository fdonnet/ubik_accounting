using Dapper;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Classifications.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public class ClassificationQueryService(AccountingDbContext ctx, ICurrentUser currentUser) : IClassificationQueryService
    {
        public async Task<IEnumerable<Classification>> GetAllAsync()
        {
            return await ctx.Classifications.ToListAsync();
        }

        public async Task<Either<IServiceAndFeatureError, Classification>> GetAsync(Guid id)
        {
            var result = await ctx.Classifications.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Classification", "Id", id.ToString())
                : result;
        }

        public async Task<Either<IServiceAndFeatureError, IEnumerable<Account>>> GetClassificationAttachedAccountsAsync(Guid id)
        {
            var accounts = (await GetAsync(id))
                .MapAsync(a =>
                {
                    var p = new DynamicParameters();
                    p.Add("@id", id);
                    p.Add("@tenantId", currentUser.TenantId);

                    var con = ctx.Database.GetDbConnection();
                    var sql = """
                              SELECT a.*
                              FROM classifications c
                              INNER JOIN account_groups ag ON c.id = ag.classification_id
                              INNER JOIN accounts_account_groups aag on aag.account_group_id = ag.id
                              INNER JOIN accounts a ON aag.account_id = a.id
                              WHERE a.tenant_id = @tenantId 
                              AND c.id = @id
                              """;

                    return con.QueryAsync<Account>(sql, p);
                });

            return await accounts;
        }

        public async Task<Either<IServiceAndFeatureError, IEnumerable<Account>>> GetClassificationMissingAccountsAsync(Guid id)
        {
            var accounts = (await GetAsync(id))
                .MapAsync(a =>
                {
                    var p = new DynamicParameters();
                    p.Add("@id", id);
                    p.Add("@tenantId", currentUser.TenantId);

                    var con = ctx.Database.GetDbConnection();
                    var sql = """
                              SELECT a1.*
                              FROM accounts a1
                              WHERE a1.tenant_id = @tenantId
                              AND a1.id NOT IN (
                                  SELECT a.id
                                  FROM classifications c
                                  INNER JOIN account_groups ag ON c.id = ag.classification_id
                                  INNER JOIN accounts_account_groups aag on aag.account_group_id = ag.id
                                  INNER JOIN accounts a ON aag.account_id = a.id
                                  WHERE c.id = @id)
                              """;

                    return con.QueryAsync<Account>(sql, p);
                });

            return await accounts;
        }

        public async Task<Either<IServiceAndFeatureError, ClassificationStatus>> GetClassificationStatusAsync(Guid id)
        {
            return (await GetClassificationMissingAccountsAsync(id))
                .Map(c =>
                {
                    ClassificationStatus status = c.Any()
                        ? new ClassificationStatus
                        {
                            Id = id,
                            IsReady = false,
                            MissingAccounts = c
                        }
                        : new ClassificationStatus
                        {
                            Id = id,
                            IsReady = true
                        };
                    return status;
                });
        }
    }
}
