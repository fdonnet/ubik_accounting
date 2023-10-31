﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.AccountGroups
{
    public class AccountGroupService : IAccountGroupService
    {
        private readonly AccountingContext _context;
        public AccountGroupService(AccountingContext ctx)
        {
            _context = ctx;

        }
        public async Task<AccountGroup> AddAsync(AccountGroup accountGroup)
        {
            accountGroup.Id = NewId.NextGuid();
            await _context.AccountGroups.AddAsync(accountGroup);
            _context.SetAuditAndSpecialFields();

            return accountGroup;
        }

        public async Task<bool> ExecuteDeleteAsync(Guid id)
        {
            await _context.AccountGroups.Where(x => x.Id == id).ExecuteDeleteAsync();
            return true;
        }

        public async Task<AccountGroup?> GetAsync(Guid id)
        {
            var accountGroup = await _context.AccountGroups.FirstOrDefaultAsync(a => a.Id == id);
            return accountGroup;
        }

        public async Task<IEnumerable<AccountGroup>> GetAllAsync()
        {
            var accountGroups = await _context.AccountGroups.ToListAsync();

            return accountGroups;
        }

        public async Task<AccountGroup?> GetWithChildAccountsAsync(Guid id)
        {
            var accountGroup = await _context.AccountGroups
                                    .Include(a => a.Accounts)
                                    .FirstOrDefaultAsync(g => g.Id == id);

            return accountGroup;
        }

        public async Task<bool> IfExistsAsync(string accountGroupCode,Guid accountGroupClassificationId)
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

        public AccountGroup Update(AccountGroup accountGroup)
        {
            _context.Entry(accountGroup).State = EntityState.Modified;
            _context.SetAuditAndSpecialFields();
            return accountGroup;
        }
    }
}
