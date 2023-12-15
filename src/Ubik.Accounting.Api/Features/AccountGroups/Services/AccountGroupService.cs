using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.AccountGroups.Errors;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public class AccountGroupService : IAccountGroupService
    {
        private readonly AccountingContext _context;
        public AccountGroupService(AccountingContext ctx)
        {
            _context = ctx;

        }
        public async Task<Either<IServiceAndFeatureError, AccountGroup>> AddAsync(AccountGroup accountGroup)
        {
            return await ValidateIfNotAlreadyExistsAsync(accountGroup).ToAsync()
                .Bind(ac => ValidateIfParentAccountGroupExistsAsync(ac).ToAsync())
                .Bind(ac => ValidateIfClassificationExistsAsync(ac).ToAsync())
                .MapAsync(async ac =>
                {
                    ac.Id = NewId.NextGuid();
                    await _context.AccountGroups.AddAsync(ac);
                    _context.SetAuditAndSpecialFields();

                    return ac;
                });
        }

        public async Task<Either<IServiceAndFeatureError, List<AccountGroup>>> DeleteAsync(Guid id)
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

        public async Task<Either<IServiceAndFeatureError, AccountGroup>> GetAsync(Guid id)
        {
            var accountGroup = await _context.AccountGroups.FirstOrDefaultAsync(a => a.Id == id);

            return accountGroup == null
                ? new AccountGroupNotFoundError(id)
                : accountGroup;
        }

        public async Task<IEnumerable<AccountGroup>> GetAllAsync()
        {
            var accountGroups = await _context.AccountGroups.ToListAsync();

            return accountGroups;
        }

        public async Task<Either<IServiceAndFeatureError, AccountGroup>> GetWithChildAccountsAsync(Guid id)
        {
            var accountGroup = await _context.AccountGroups
                                    .Include(a => a.Accounts)
                                    .FirstOrDefaultAsync(g => g.Id == id);



            if (accountGroup is null)
                return new AccountGroupNotFoundError(id);

            accountGroup.Accounts ??= new List<Account>();

            return accountGroup;
        }

        public async Task<Either<IServiceAndFeatureError, AccountGroup>> UpdateAsync(AccountGroup accountGroup)
        {
            return await GetAsync(accountGroup.Id).ToAsync()
                .Map(ag => ag = accountGroup.ToAccountGroup(ag))
                .Bind(ag => ValidateIfNotAlreadyExistsWithOtherIdAsync(ag).ToAsync())
                .Bind(ag => ValidateIfParentAccountGroupExistsAsync(ag).ToAsync())
                .Bind(ag => ValidateIfClassificationExistsAsync(ag).ToAsync())
                .Map(ag =>
                {
                    _context.Entry(ag).State = EntityState.Modified;
                    _context.SetAuditAndSpecialFields();

                    return ag;
                });
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> ValidateIfParentAccountGroupExistsAsync(AccountGroup accountGroup)
        {
            return accountGroup.ParentAccountGroupId != null
                ? await _context.AccountGroups.AnyAsync(a => a.Id == (Guid)accountGroup.ParentAccountGroupId)
                    ? accountGroup
                    : new AccountGroupParentNotFoundError((Guid)accountGroup.ParentAccountGroupId)
                : (Either<IServiceAndFeatureError, AccountGroup>)accountGroup;
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> ValidateIfClassificationExistsAsync(AccountGroup accountGroup)
        {
            return await _context.Classifications.AnyAsync(a => a.Id == accountGroup.ClassificationId)
                ? accountGroup
                : new AccountGroupClassificationNotFoundError(accountGroup.ClassificationId);
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

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> ValidateIfNotAlreadyExistsAsync(AccountGroup accountGroup)
        {
            var exists = await _context.AccountGroups.AnyAsync(a => a.Code == accountGroup.Code
                        && a.ClassificationId == accountGroup.ClassificationId);

            return exists
                ? new AccountGroupAlreadyExistsError(accountGroup.Code, accountGroup.ClassificationId)
                : accountGroup;
        }

        private async Task<Either<IServiceAndFeatureError, AccountGroup>> ValidateIfNotAlreadyExistsWithOtherIdAsync(AccountGroup accountGroup)
        {
            var exists = await _context.AccountGroups.AnyAsync(a => a.Code == accountGroup.Code
                        && a.ClassificationId == accountGroup.ClassificationId
                        && a.Id != accountGroup.Id);

            return exists
                ? new AccountGroupAlreadyExistsError(accountGroup.Code, accountGroup.ClassificationId)
                : accountGroup;
        }
    }
}
