using Dapper;
using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Classifications.CustomPoco;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public class ClassificationService(AccountingDbContext ctx, ICurrentUser currentUser) : IClassificationService
    {
        public async Task<IEnumerable<Classification>> GetAllAsync()
        {
            var result = await ctx.Classifications.ToListAsync();

            return result;
        }

        public async Task<Either<IServiceAndFeatureError, Classification>> GetAsync(Guid id)
        {
            var result = await ctx.Classifications.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Classification","Id",id.ToString())
                : result;
        }

        /// <summary>
        /// Dapper to get all the account linked to a classification (Postgres)
        /// TODO:change tenant id array to selected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Either<IServiceAndFeatureError, IEnumerable<Account>>> GetClassificationAccountsAsync(Guid id)
        {
            //Get the classification and if exists, map the accounts
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

        /// <summary>
        /// TODO:change tenant id array to selected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Either<IServiceAndFeatureError, IEnumerable<Account>>> GetClassificationAccountsMissingAsync(Guid id)
        {
            //Get the classification and if exists, return all missing accounts
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

                    return  con.QueryAsync<Account>(sql, p);
                });

            return await accounts;
        }

        public async Task<Either<IServiceAndFeatureError, ClassificationStatus>> GetClassificationStatusAsync(Guid id)
        {
            return (await GetClassificationAccountsMissingAsync(id))
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

        public async Task<Either<IServiceAndFeatureError, Classification>> AddAsync(Classification classification)
        {
            return await ValidateIfNotAlreadyExistsAsync(classification).ToAsync()
                .MapAsync(async c =>
                {
                    c.Id = NewId.NextGuid();
                    await ctx.Classifications.AddAsync(c);
                    ctx.SetAuditAndSpecialFields();

                    return c;
                });
        }

        public async Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteAsync(Guid id)
        {
            return await GetAsync(id).ToAsync()
                .MapAsync(async c =>
                {
                    using var transaction = ctx.Database.BeginTransaction();

                    //Clean all account groups structure
                    var firstLvlAccountGroups = await GetFirstLvlAccountGroupsAsync(id);
                    var deletedAccountGroups = new List<AccountGroup>();

                    foreach (var ag in firstLvlAccountGroups)
                    {
                        deletedAccountGroups.Add(ag);
                        await DeleteAllChildrenAccountGroupsAsync(id, deletedAccountGroups);
                        await ctx.AccountGroups.Where(x => x.Id == ag.Id).ExecuteDeleteAsync();
                    }

                    //Delete classification
                    await ctx.Classifications.Where(x => x.Id == id).ExecuteDeleteAsync();

                    transaction.Commit();
                    return deletedAccountGroups;
                });
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> ValidateIfNotAlreadyExistsAsync(Classification classification)
        {
            var exists = await ctx.Classifications.AnyAsync(a => a.Code == classification.Code);

            return exists
                ? new ResourceAlreadyExistsError("Classification","Code",classification.Code)
                : classification;
        }

        private async Task<IEnumerable<AccountGroup>> GetFirstLvlAccountGroupsAsync(Guid classificationId)
        {
            return await ctx.AccountGroups
                                    .Where(ag => ag.ClassificationId == classificationId
                                                && ag.ParentAccountGroupId == null).ToListAsync();
        }

        private async Task DeleteAllChildrenAccountGroupsAsync(Guid id, List<AccountGroup> deletedAccountGroups)
        {
            var children = await ctx.AccountGroups.Where(ag => ag.ParentAccountGroupId == id).ToListAsync();

            foreach (var child in children)
            {
                await DeleteAllChildrenAccountGroupsAsync(child.Id, deletedAccountGroups);
                deletedAccountGroups.Add(child);
                await ctx.AccountGroups.Where(x => x.Id == child.Id).ExecuteDeleteAsync();
            }
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Classification classification)
        {
            var exists = await ctx.Classifications.AnyAsync(a => a.Code == classification.Code && a.Id != classification.Id);

            return exists
                ? new ResourceAlreadyExistsError("Classification", "Code", classification.Code)
                : classification;
        }
    }
}
