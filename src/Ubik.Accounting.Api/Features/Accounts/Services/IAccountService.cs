using LanguageExt;
using Ubik.Accounting.Api.Features.Accounts.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public interface IAccountService
    {
        public Task<Either<IServiceAndFeatureError, AccountAccountGroup>> AddInAccountGroupAsync(AccountAccountGroup accountAccountGroup);
        public Task<Either<IServiceAndFeatureError, AccountAccountGroup>> DeleteFromAccountGroupAsync(Guid id, Guid accountGroupId);
    }
}
