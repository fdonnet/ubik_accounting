using LanguageExt;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.Accounts.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.Accounts.Services
{
    public interface IAccountCommandService
    {
        public Task<Either<IServiceAndFeatureError, Account>> AddAsync(AddAccountCommand command);
        public Task<Either<IServiceAndFeatureError, Account>> UpdateAsync(UpdateAccountCommand command);
        public Task<Either<IServiceAndFeatureError, bool>> DeleteAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, AccountAccountGroup>> AddInAccountGroupAsync(AddAccountInAccountGroupCommand command);
        public Task<Either<IServiceAndFeatureError, AccountAccountGroup>> DeleteFromAccountGroupAsync(DeleteAccountInAccountGroupCommand command);
    }
}
