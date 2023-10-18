﻿using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
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
        public async Task<AccountGroup> AddAsync(AccountGroup accountGroup)
        {
            await _context.AccountGroups.AddAsync(accountGroup);
            await _context.SaveChangesAsync();

            return accountGroup;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _context.AccountGroups.Where(x => x.Id == id).ExecuteDelete();
            await _context.SaveChangesAsync();
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

        public async Task<bool> IfExistsAsync(string accountGroupCode)
        {
            return await _context.AccountGroups.AnyAsync(a => a.Code == accountGroupCode);
        }

        public async Task<bool> IfExistsWithDifferentIdAsync(string accountGroupCode, Guid currentId)
        {
            return await _context.AccountGroups.AnyAsync(a => a.Code == accountGroupCode && a.Id != currentId);
        }

        public async Task<AccountGroup> UpdateAsync(AccountGroup accountGroup)
        {
            _context.Entry(accountGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return accountGroup;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var err = new CustomError()
                {
                    ErrorCode = "ACCOUNTGROUP_CONFLICT",
                    ErrorFriendlyMessage = "You don't have the last version or this account group has been removed, refresh your data before updating.",
                    ErrorValueDetails = "Version",
                };
                var conflict = new AccountUpdateDbConcurrencyException(accountGroup.Version, ex)
                {
                    CustomErrors = new List<CustomError> { err }
                };

                throw conflict;
            }
        }
    }
}