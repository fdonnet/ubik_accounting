using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Accounts.Services
{
    public interface IAccountCommandService
    {
        public Task<Either<IServiceAndFeatureError, Account>> AddAsync(AddAccountCommand command);
        public Task<Either<IServiceAndFeatureError, Account>> UpdateAsync(UpdateAccountCommand command);
        public Task<Either<IServiceAndFeatureError, bool>> DeleteAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, AccountAccountGroup>> AddInAccountGroupAsync(AddAccountInAccountGroupCommand command);
    }
}
