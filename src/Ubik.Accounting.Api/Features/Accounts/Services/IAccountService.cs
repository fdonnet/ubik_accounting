using LanguageExt;
using LanguageExt.Common;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public interface IAccountService
    {
        public Task<IEnumerable<Account>> GetAllAsync();
        public Task<ResultT<Account>> GetAsync(Guid id);
        public Task<bool> IfExistsAsync(string accountCode);
        public Task<bool> IfExistsWithDifferentIdAsync(string accountCode, Guid currentId);
        public Task<Account> AddAsync(Account account);
        public Account Update(Account account);
        public Task<bool> ExecuteDeleteAsync(Guid id);
        public Task<bool> IfExistsCurrencyAsync(Guid currencyId);

    }
}
