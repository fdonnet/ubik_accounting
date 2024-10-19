using LanguageExt;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Services
{
    public interface IAccountGroupCommandService
    {
        public Task<Either<IServiceAndFeatureError, AccountGroup>> AddAsync(AddAccountGroupCommand command);
        public Task<Either<IServiceAndFeatureError, AccountGroup>> UpdateAsync(UpdateAccountGroupCommand command);
    }
}
