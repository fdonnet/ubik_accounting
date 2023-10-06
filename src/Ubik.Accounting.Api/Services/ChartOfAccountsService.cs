using LanguageExt.ClassInstances.Const;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Dto.Mappers;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Services
{
    public interface IChartOfAccountsService
    {
        public Task<IEnumerable<AccountDto>> GetAccountsAsync();
        public Task<IEnumerable<AccountWithAccountGroupDto>> GetAccountsWithAccountGroupAsync();
        public Task<Account> AddAccountAsync(AccountDtoForAdd account);
    }

    public class ChartOfAccountsService : IChartOfAccountsService
    {
        private readonly AccountingContext _context;
        public ChartOfAccountsService(AccountingContext ctx)
        {
            _context = ctx;
        }

        public async Task<IEnumerable<AccountDto>> GetAccountsAsync()
        {
            var accounts = await _context.Accounts.ToListAsync();

            return accounts.Select(a => AccountMapper.ToAccountDto(a));
        }

        public async Task<IEnumerable<AccountWithAccountGroupDto>> GetAccountsWithAccountGroupAsync()
        {
            var accounts = await _context.Accounts
                                .Include(a => a.Group)
                                .ToListAsync();

            return accounts.Select(a => AccountMapper.ToAccountWithAccountGroup(a));
        }

        public async Task<Account> AddAccountAsync(AccountDtoForAdd accountDto)
        {
            var account = AccountMapper.ToAccount(accountDto);
            account.Version = Guid.NewGuid();

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return account;
        }

    }
}
