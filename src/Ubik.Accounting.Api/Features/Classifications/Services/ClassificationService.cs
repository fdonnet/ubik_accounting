using Dapper;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.SomeHelp;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Features.Classifications.Exceptions;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Api.Features.Classifications.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
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

        public async Task<bool> IfExistsAsync(Guid id)
        {
            return await _context.Classifications.AnyAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Classification>> GetAllAsync()
        {
            var result = await _context.Classifications.ToListAsync();

            return result;
        }

        public async Task<Either<IServiceAndFeatureException, Classification>> GetAsync(Guid id)
        {
            var result = await _context.Classifications.FirstOrDefaultAsync(a => a.Id == id);

            return result == null
                ? new ClassificationNotFoundException(id)
                : result;
        }

        /// <summary>
        /// Dapper to get all the account linked to a classification (Postgres)
        /// TODO:change tenant id array to selected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Either<IServiceAndFeatureException, IEnumerable<Account>>> GetClassificationAccountsAsync(Guid id)
        {
            if ((await GetAsync(id)).IsLeft)
                return new ClassificationNotFoundException(id);

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

            return Prelude.Right((await con.QueryAsync<Account>(sql, p)));

        }

        /// <summary>
        /// TODO:change tenant id array to selected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Either<IServiceAndFeatureException, IEnumerable<Account>>> GetClassificationAccountsMissingAsync(Guid id)
        {
            if ((await GetAsync(id)).IsLeft)
                return new ClassificationNotFoundException(id);

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

            return Prelude.Right(await con.QueryAsync<Account>(sql, p));
        }

        public async Task<Either<IServiceAndFeatureException, ClassificationStatus>> GetClassificationStatusAsync(Guid id)
        {
            var missingAccount = await GetClassificationAccountsMissingAsync(id);

            if (missingAccount.IsLeft)
                return new ClassificationNotFoundException(id);

            var classificationStatus = missingAccount.Match(
                Right: c =>
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
                            IsReady = false
                        };
                    return status;
                },
                Left: err => throw new Exception("Cannot be in left state at this point"));

            return classificationStatus;
        }

        public async Task<bool> IfExistsAsync(string code)
        {
            return await _context.Classifications.AnyAsync(a => a.Code == code);
        }

        public async Task<bool> IfExistsWithDifferentIdAsync(string code, Guid currentId)
        {
            return await _context.Classifications.AnyAsync(a => a.Code == code && a.Id != currentId);
        }

        public async Task<Either<IServiceAndFeatureException, Classification>> AddAsync(Classification classification)
        {
            var exist = await IfExistsAsync(classification.Code);
            if (exist)
                return new ClassificationAlreadyExistsException(classification.Code);

            classification.Id = NewId.NextGuid();
            await _context.Classifications.AddAsync(classification);
            _context.SetAuditAndSpecialFields();

            return classification;
        }

        public async Task<Either<IServiceAndFeatureException, Classification>> UpdateAsync(Classification classification)
        {
            //Is found
            var testPresent = await GetAsync(classification.Id);
            if (testPresent.IsLeft)
                return testPresent;

            var toUpdate = testPresent.IfLeft(err => default!);

            //Classification code already exists with other id
            if (await IfExistsWithDifferentIdAsync(classification.Code, classification.Id))
                return new ClassificationAlreadyExistsException(classification.Code);

            //Save
            toUpdate = classification.ToClassification(toUpdate);

            _context.Entry(toUpdate).State = EntityState.Modified;
            _context.SetAuditAndSpecialFields();

            return toUpdate;
        }

        public async Task<Either<IServiceAndFeatureException, IEnumerable<AccountGroup>>> DeleteAsync(Guid id)
        {
            var classification = await GetAsync(id);

            if (classification.IsRight)
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
            }
            else
            {
                return new ClassificationNotFoundException(id);
            }
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
    }
}
