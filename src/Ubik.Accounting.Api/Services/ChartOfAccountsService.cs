using LanguageExt.Common;
using LanguageExt.Pipes;
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
        public Task<Result<bool>> UpdateAccountAsync(Guid currentId, AccountDto account);
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
                    ErrorValueDetails = "Code"
                };
                return new Result<Account>(alreadyExists);
            }

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task<Result<bool>> UpdateAccountAsync(Guid currentId, AccountDto accountDto)
        {
            //Not correct ID
            if (currentId != accountDto.Id)
            {
                var notSameId = new ServiceException()
                {
                    ErrorCode = "ACCOUNT_ID_NOTMATCH",
                    ExceptionType = ServiceExceptionType.BadParams,
                    ErrorFriendlyMessage = "The account Id provided doesn't match the account information sent for update.",
                    ErrorValueDetails = "Id"
                };
                return new Result<bool>(notSameId);
            }


            //Check if the record exists
            var accountToUpdate = await _context.Accounts
                                        .AsNoTracking()
                                        .SingleOrDefaultAsync(x => x.Id == currentId);

            if(accountToUpdate == null)
            {
                var notExistsForUpdate = new ServiceException()
                {
                    ErrorCode = "ACCOUNT_NOT_FOUND",
                    ExceptionType = ServiceExceptionType.NotFound,
                    ErrorFriendlyMessage = "The account doesn't exist. Id not found.",
                    ErrorValueDetails = "Id",
                };
                return new Result<bool>(notExistsForUpdate);
            }

            //Check if the account code already exists
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
                return new Result<bool>(alreadyExists);
            }

            accountToUpdate = accountDto.ToAccount(accountToUpdate);

            _context.Entry(accountToUpdate).State = EntityState.Modified;
            
            //TODO: conccurrency check
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
