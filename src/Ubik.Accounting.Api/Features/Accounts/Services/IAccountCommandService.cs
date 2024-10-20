using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public interface IAccountCommandService
    {
        public Task<Either<IServiceAndFeatureError, Account>> AddAsync(Account account);
    }
}
