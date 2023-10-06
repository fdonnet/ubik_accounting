using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Dto.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Services
{
    public interface IChartOfAccountsService
    {
        public Task<IEnumerable<AccountDto>> GetAccountsAsync();
        public Task<IEnumerable<AccountWithAccountGroupDto>> GetAccountsWithAccountGroupAsync();
        public Task<Result<Account>> AddAccountAsync(AccountDtoForAdd account);
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

        public async Task<Result<Account>> AddAccountAsync(AccountDtoForAdd accountDto)
        {
            var account = AccountMapper.ToAccount(accountDto);

            bool exists = await _context.Accounts.AnyAsync(a => a.Code == accountDto.Code);
            if (exists)
            {
                var alreadyExists = new ServiceException()
                {
                    ErrorCode = "ACCOUNT_ALREADY_EXISTS",
                    ExceptionType = ServiceExceptionType.AlreadyExists,
                    ErrorFriendlyMessage = "The account already exists. Code field needs to be unique.",
                    ErrorValueDetails = "Code",
                };
                return new Result<Account>(alreadyExists);
            }

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return account;
        }
    }
}
