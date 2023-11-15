using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public class AccountGroupService : IAccountGroupService
    {
        private readonly AccountingContext _context;
        public AccountGroupService(AccountingContext ctx)
        {
            _context = ctx;

        }
        public async Task<Either<IServiceAndFeatureException, AccountGroup>> AddAsync(AccountGroup accountGroup)
        {
            return await ValidateIfNotAlreadyExistsAsync(accountGroup).ToAsync()
                .Bind(ac => ValidateIfParentAccountGroupExists(ac).ToAsync())
                .Bind(ac => ValidateIfClassificationExists(ac).ToAsync())
                .MapAsync(async ac =>
                {
                    ac.Id = NewId.NextGuid();
                    await _context.AccountGroups.AddAsync(ac);
                    _context.SetAuditAndSpecialFields();

                    return ac;
                });
        }

        public async Task<Either<IServiceAndFeatureException, List<AccountGroup>>> DeleteAsync(Guid id)
        {
            return await GetAsync(id).ToAsync()
                .MapAsync(async ag =>
                {
                    using var transaction = _context.Database.BeginTransaction();
                    var deletedAccountGroups = new List<AccountGroup>();
                    await DeleteAllChildrenOfAsync(id, deletedAccountGroups);
                    await _context.AccountGroups.Where(x => x.Id == id).ExecuteDeleteAsync();
                    deletedAccountGroups.Add(ag);

                    transaction.Commit();
                    return deletedAccountGroups;
                });
        }

        public async Task<Either<IServiceAndFeatureException, AccountGroup>> GetAsync(Guid id)
        {
            var accountGroup = await _context.AccountGroups.FirstOrDefaultAsync(a => a.Id == id);

            return accountGroup == null
                ? new AccountGroupNotFoundException(id)
                : accountGroup;
        }

        public async Task<IEnumerable<AccountGroup>> GetAllAsync()
        {
            var accountGroups = await _context.AccountGroups.ToListAsync();

            return accountGroups;
        }

        public async Task<Either<IServiceAndFeatureException, AccountGroup>> GetWithChildAccountsAsync(Guid id)
        {
            var accountGroup = await _context.AccountGroups
                                    .Include(a => a.Accounts)
                                    .FirstOrDefaultAsync(g => g.Id == id);



            if (accountGroup is null)
                return new AccountGroupNotFoundException(id);

            accountGroup.Accounts ??= new List<Account>();

            return accountGroup;
        }

        public async Task<Either<IServiceAndFeatureException, AccountGroup>> UpdateAsync(AccountGroup accountGroup)
        {
            return await GetAsync(accountGroup.Id).ToAsync()
                .Map(ag => ag = accountGroup.ToAccountGroup(ag))
                .Bind(ag => ValidateIfNotAlreadyExistsWithOtherIdAsync(ag).ToAsync())
                .Bind(ag => ValidateIfParentAccountGroupExists(ag).ToAsync())
                .Bind(ag => ValidateIfClassificationExists(ag).ToAsync())
                .Map(ag =>
                {
                    _context.Entry(ag).State = EntityState.Modified;
                    _context.SetAuditAndSpecialFields();

                    return ag;
                });
        }

        private async Task<Either<IServiceAndFeatureException, AccountGroup>> ValidateIfParentAccountGroupExists(AccountGroup accountGroup)
        {
            return accountGroup.ParentAccountGroupId != null
                ? await _context.AccountGroups.AnyAsync(a => a.Id == (Guid)accountGroup.ParentAccountGroupId)
                    ? accountGroup
                    : new AccountGroupParentNotFoundException((Guid)accountGroup.ParentAccountGroupId)
                : (Either<IServiceAndFeatureException, AccountGroup>)accountGroup;
        }

        private async Task<Either<IServiceAndFeatureException, AccountGroup>> ValidateIfClassificationExists(AccountGroup accountGroup)
        {
            return await _context.Classifications.AnyAsync(a => a.Id == accountGroup.ClassificationId)
                ? accountGroup
                : new AccountGroupClassificationNotFound(accountGroup.ClassificationId);
        }

        private async Task DeleteAllChildrenOfAsync(Guid id, List<AccountGroup> deletedAccountGroups)
        {
            var children = await _context.AccountGroups.Where(ag => ag.ParentAccountGroupId == id).ToListAsync();

            foreach (var child in children)
            {
                await DeleteAllChildrenOfAsync(child.Id, deletedAccountGroups);
                deletedAccountGroups.Add(child);
                await _context.AccountGroups.Where(x => x.Id == child.Id).ExecuteDeleteAsync();
            }
        }

        private async Task<Either<IServiceAndFeatureException, AccountGroup>> ValidateIfNotAlreadyExistsAsync(AccountGroup accountGroup)
        {
            var exists = await _context.AccountGroups.AnyAsync(a => a.Code == accountGroup.Code
                        && a.ClassificationId == accountGroup.ClassificationId);

            return exists
                ? new AccountGroupAlreadyExistsException(accountGroup.Code, accountGroup.ClassificationId)
                : accountGroup;
        }

        private async Task<Either<IServiceAndFeatureException, AccountGroup>> ValidateIfNotAlreadyExistsWithOtherIdAsync(AccountGroup accountGroup)
        {
            var exists = await _context.AccountGroups.AnyAsync(a => a.Code == accountGroup.Code
                        && a.ClassificationId == accountGroup.ClassificationId
                        && a.Id != accountGroup.Id);

            return exists
                ? new AccountGroupAlreadyExistsException(accountGroup.Code, accountGroup.ClassificationId)
                : accountGroup;
        }
    }
}
