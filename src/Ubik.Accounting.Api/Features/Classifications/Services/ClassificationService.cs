using Dapper;
using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.AccountGroups.Errors;
using Ubik.Accounting.Api.Features.Classifications.Errors;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Api.Features.Classifications.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public class ClassificationService : IClassificationService
    {
        private readonly AccountingContext _context;
        private readonly ICurrentUserService _userService;
        public ClassificationService(AccountingContext ctx, ICurrentUserService userService)
        {
            _context = ctx;
            _userService = userService;
        }

        public async Task<IEnumerable<Classification>> GetAllAsync()
        {
            var result = await _context.Classifications.ToListAsync();

            return result;
        }

        public async Task<Either<IServiceAndFeatureError, Classification>> GetAsync(Guid id)
        {
            var result = await _context.Classifications.FirstOrDefaultAsync(a => a.Id == id);

            return result == null
                ? new ClassificationNotFoundError(id)
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
                    p.Add("@tenantId", _userService.CurrentUser.TenantIds[0]);

                    var con = _context.Database.GetDbConnection();
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
                    p.Add("@tenantId", _userService.CurrentUser.TenantIds[0]);

                    var con = _context.Database.GetDbConnection();
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
                    await _context.Classifications.AddAsync(c);
                    _context.SetAuditAndSpecialFields();

                    return c;
                });
        }

        public async Task<Either<IServiceAndFeatureError, Classification>> UpdateAsync(Classification classification)
        {
            return await GetAsync(classification.Id).ToAsync()
                .Map(c => c = classification.ToClassification(c))
                .Bind(c => ValidateIfNotAlreadyExistsWithOtherIdAsync(c).ToAsync())
                .Map(c =>
                {
                    _context.Entry(c).State = EntityState.Modified;
                    _context.SetAuditAndSpecialFields();

                    return c;
                });
        }

        public async Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteAsync(Guid id)
        {
            return await GetAsync(id).ToAsync()
                .MapAsync(async c =>
                {
                    using var transaction = _context.Database.BeginTransaction();

                    //Clean all account groups structure
                    var firstLvlAccountGroups = await GetFirstLvlAccountGroups(id);
                    var deletedAccountGroups = new List<AccountGroup>();

                    foreach (var ag in firstLvlAccountGroups)
                    {
                        deletedAccountGroups.Add(ag);
                        await DeleteAllChildrenAccountGroupsAsync(id, deletedAccountGroups);
                        await _context.AccountGroups.Where(x => x.Id == ag.Id).ExecuteDeleteAsync();
                    }

                    //Delete classification
                    await _context.Classifications.Where(x => x.Id == id).ExecuteDeleteAsync();

                    transaction.Commit();
                    return deletedAccountGroups;
                });
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> ValidateIfNotAlreadyExistsAsync(Classification classification)
        {
            var exists = await _context.Classifications.AnyAsync(a => a.Code == classification.Code);

            return exists
                ? new ClassificationAlreadyExistsError(classification.Code)
                : classification;
        }

        private async Task<IEnumerable<AccountGroup>> GetFirstLvlAccountGroups(Guid classificationId)
        {
            return await _context.AccountGroups
                                    .Where(ag => ag.ClassificationId == classificationId
                                                && ag.ParentAccountGroupId == null).ToListAsync();
        }

        private async Task DeleteAllChildrenAccountGroupsAsync(Guid id, List<AccountGroup> deletedAccountGroups)
        {
            var children = await _context.AccountGroups.Where(ag => ag.ParentAccountGroupId == id).ToListAsync();

            foreach (var child in children)
            {
                await DeleteAllChildrenAccountGroupsAsync(child.Id, deletedAccountGroups);
                deletedAccountGroups.Add(child);
                await _context.AccountGroups.Where(x => x.Id == child.Id).ExecuteDeleteAsync();
            }
        }

        private async Task<Either<IServiceAndFeatureError, Classification>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Classification classification)
        {
            var exists = await _context.Classifications.AnyAsync(a => a.Code == classification.Code && a.Id != classification.Id);

            return exists
                ? new ClassificationAlreadyExistsError(classification.Code)
                : classification;
        }
    }
}
