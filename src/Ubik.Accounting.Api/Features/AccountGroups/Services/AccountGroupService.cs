using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
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
            //Already exists
            var exist = await IfExistsAsync(accountGroup.Code, accountGroup.ClassificationId);
            if (exist)
                return new AccountGroupAlreadyExistsException(accountGroup.Code,
                    accountGroup.ClassificationId);

            //Validate dependencies
            var validated = await ValidateRelationsAsync(accountGroup);

            if (validated.IsLeft)
                return validated;

            accountGroup.Id = NewId.NextGuid();
            await _context.AccountGroups.AddAsync(accountGroup);
            _context.SetAuditAndSpecialFields();

            return accountGroup;
        }

        public async Task<Either<IServiceAndFeatureException, IEnumerable<AccountGroup>>> DeleteAsync(Guid id)
        {
            var accountGrp = await GetAsync(id);

            if(accountGrp.IsRight)
            {
                using var transaction = _context.Database.BeginTransaction();
                var deletedAccountGroups = new List<AccountGroup>();
                await DeleteAllChildrenOfAsync(id, deletedAccountGroups);
                await _context.AccountGroups.Where(x => x.Id == id).ExecuteDeleteAsync();
                deletedAccountGroups.Add(accountGrp.IfLeft(x=>default!));

                transaction.Commit();
                return deletedAccountGroups;
            }
            else
            {
                return new AccountGroupNotFoundException(id);
            }
        }

        private async Task DeleteAllChildrenOfAsync(Guid id, List<AccountGroup> deletedAccountGroups)
        {
            var children = await _context.AccountGroups.Where(ag => ag.ParentAccountGroupId == id).ToListAsync();

            foreach(var child in children)
            {
                await DeleteAllChildrenOfAsync(child.Id,deletedAccountGroups);
                deletedAccountGroups.Add(child);    
                await _context.AccountGroups.Where(x => x.Id == child.Id).ExecuteDeleteAsync();
            }
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

        public async Task<bool> IfExistsAsync(string accountGroupCode, Guid accountGroupClassificationId)
        {
            return await _context.AccountGroups.AnyAsync(a => a.Code == accountGroupCode
                        && a.ClassificationId == accountGroupClassificationId);
        }

        public async Task<bool> HasAnyChildAccountGroups(Guid Id)
        {
            return await _context.AccountGroups.AnyAsync(a => a.ParentAccountGroupId == Id);
        }

        public async Task<bool> HasAnyChildAccounts(Guid Id)
        {
            return await _context.AccountsAccountGroups.AnyAsync(a => a.AccountGroupId == Id);
        }

        public async Task<bool> IfExistsAsync(Guid accountGroupId)
        {
            return await _context.AccountGroups.AnyAsync(a => a.Id == accountGroupId);
        }

        public async Task<bool> IfExistsWithDifferentIdAsync(string accountGroupCode, Guid accountGroupClassificationId, Guid currentId)
        {
            return await _context.AccountGroups.AnyAsync(a => a.Code == accountGroupCode
                        && a.ClassificationId == accountGroupClassificationId
                        && a.Id != currentId);

        }

        public async Task<Either<IServiceAndFeatureException, AccountGroup>> UpdateAsync(AccountGroup accountGroup)
        {
            //Is found
            var testPresent = await GetAsync(accountGroup.Id);
            if (testPresent.IsLeft)
                return testPresent;

            var toUpdate = testPresent.IfLeft(err => default!);

            //Group code already exists in the same classification
            var alreadyExistsWithOtherId = await IfExistsWithDifferentIdAsync(accountGroup.Code,
                accountGroup.ClassificationId, accountGroup.Id);

            if (alreadyExistsWithOtherId)
                return new AccountGroupAlreadyExistsException(accountGroup.Code, accountGroup.ClassificationId);

            //Validate dependencies
            var validated = await ValidateRelationsAsync(accountGroup);
            if (validated.IsLeft)
                return validated;

            //Save
            toUpdate = accountGroup.ToAccountGroup(toUpdate);

            _context.Entry(toUpdate).State = EntityState.Modified;
            _context.SetAuditAndSpecialFields();

            return toUpdate;
        }

        public async Task<bool> IfClassificationExists(Guid accountGroupClassificationId)
        {
            return await _context.Classifications.AnyAsync(a => a.Id == accountGroupClassificationId);
        }

        private async Task<Either<IServiceAndFeatureException, AccountGroup>> ValidateRelationsAsync(AccountGroup accountGroup)
        {
            if (accountGroup.ParentAccountGroupId != null)
            {
                var parentAccountExists = await IfExistsAsync((Guid)accountGroup.ParentAccountGroupId);

                if (!parentAccountExists)
                    return new AccountGroupParentNotFoundException((Guid)accountGroup.ParentAccountGroupId);
            }

            var classificationExists = await IfClassificationExists(accountGroup.ClassificationId);

            return !classificationExists
                ? new AccountGroupClassificationNotFound(accountGroup.ClassificationId)
                :  accountGroup;
        }
    }
}
