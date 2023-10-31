using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups
{
    public class AccountGroupService : IAccountGroupService
    {
        private readonly AccountingContext _context;
        public AccountGroupService(AccountingContext ctx)
        {
            _context = ctx;

        }
        public async Task<ResultT<AccountGroup>> AddAsync(AccountGroup accountGroup)
        {
            var exist = await IfExistsAsync(accountGroup.Code, accountGroup.AccountGroupClassificationId);
            if (exist)
                return new ResultT<AccountGroup>
                {
                    IsSuccess = false,
                    Exception = new AccountGroupAlreadyExistsException(
                            accountGroup.Code, accountGroup.AccountGroupClassificationId)
                };

            if (accountGroup.ParentAccountGroupId != null)
            {
                var parentAccountExists = await IfExistsAsync((Guid)accountGroup.ParentAccountGroupId);

                if (!parentAccountExists)
                    return new ResultT<AccountGroup>
                    {
                        IsSuccess = false,
                        Exception = new AccountGroupParentNotFoundException((Guid)accountGroup.ParentAccountGroupId)
                    };
            }

            accountGroup.Id = NewId.NextGuid();
            await _context.AccountGroups.AddAsync(accountGroup);
            _context.SetAuditAndSpecialFields();

            return new ResultT<AccountGroup>() { IsSuccess = true, Result = accountGroup };
        }

        public async Task<ResultT<bool>> ExecuteDeleteAsync(Guid id)
        {
            var accountGrp = await GetAsync(id);

            if (accountGrp.IsSuccess)
            {
                await _context.AccountGroups.Where(x => x.Id == id).ExecuteDeleteAsync();
                return new ResultT<bool>
                {
                    IsSuccess = true,
                    Result = true,
                };
            }
            else
                return new ResultT<bool>
                {
                    IsSuccess = false,
                    Exception = new AccountGroupNotFoundException(id)
                };
        }

        public async Task<ResultT<AccountGroup>> GetAsync(Guid id)
        {
            var accountGroup = await _context.AccountGroups.FirstOrDefaultAsync(a => a.Id == id);

            return accountGroup == null
                ? new ResultT<AccountGroup>() { IsSuccess = false, Exception = new AccountGroupNotFoundException(id) }
                : new ResultT<AccountGroup>() { IsSuccess = true, Result = accountGroup };
        }

        public async Task<IEnumerable<AccountGroup>> GetAllAsync()
        {
            var accountGroups = await _context.AccountGroups.ToListAsync();

            return accountGroups;
        }

        public async Task<ResultT<AccountGroup>> GetWithChildAccountsAsync(Guid id)
        {
            var accountGroup = await _context.AccountGroups
                                    .Include(a => a.Accounts)
                                    .FirstOrDefaultAsync(g => g.Id == id);

            if (accountGroup is null)
                return new ResultT<AccountGroup>() { IsSuccess = false, Exception = new AccountGroupNotFoundException(id) };

            accountGroup.Accounts ??= new List<Account>();

            return new ResultT<AccountGroup>() { IsSuccess = true, Result = accountGroup };
        }

        public async Task<bool> IfExistsAsync(string accountGroupCode, Guid accountGroupClassificationId)
        {
            return await _context.AccountGroups.AnyAsync(a => a.Code == accountGroupCode
                        && a.AccountGroupClassificationId == accountGroupClassificationId);
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
                        && a.AccountGroupClassificationId == accountGroupClassificationId
                        && a.Id != currentId);

        }

        public async Task<ResultT<AccountGroup>> UpdateAsync(AccountGroup accountGroup)
        {
            var present = await GetAsync(accountGroup.Id);

            if (!present.IsSuccess)
                return present;

            var toUpdate = present.Result;

            var alreadyExistsWithOtherId = await IfExistsWithDifferentIdAsync(accountGroup.Code,
                accountGroup.AccountGroupClassificationId, accountGroup.Id);

            if (alreadyExistsWithOtherId)
                return new ResultT<AccountGroup>
                {
                    IsSuccess = false,
                    Exception = new AccountGroupAlreadyExistsException(accountGroup.Code, accountGroup.AccountGroupClassificationId)
                };

            if (accountGroup.ParentAccountGroupId != null)
            {
                var parentAccountGroupExists = await IfExistsAsync((Guid)accountGroup.ParentAccountGroupId);
                if (!parentAccountGroupExists)
                    return new ResultT<AccountGroup>
                    {
                        IsSuccess = false,
                        Exception = new AccountGroupParentNotFoundException((Guid)accountGroup.ParentAccountGroupId)
                    };
            }

            toUpdate = accountGroup.ToAccountGroup(toUpdate);

            _context.Entry(accountGroup).State = EntityState.Modified;
            _context.SetAuditAndSpecialFields();

            return new ResultT<AccountGroup> { IsSuccess = true, Result = toUpdate };
        }
    }
}
