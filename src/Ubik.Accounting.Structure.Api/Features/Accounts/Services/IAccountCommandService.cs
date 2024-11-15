using LanguageExt;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.Accounts.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.Accounts.Services
{
    public interface IAccountCommandService
    {
        public Task<Either<IFeatureError, Account>> AddAsync(AddAccountCommand command);
        public Task<Either<IFeatureError, Account>> UpdateAsync(UpdateAccountCommand command);
        public Task<Either<IFeatureError, bool>> DeleteAsync(Guid id);
        public Task<Either<IFeatureError, AccountAccountGroup>> AddInAccountGroupAsync(AddAccountInAccountGroupCommand command);
        public Task<Either<IFeatureError, AccountAccountGroup>> DeleteFromAccountGroupAsync(DeleteAccountInAccountGroupCommand command);
    }
}
