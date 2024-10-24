using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Commands;

namespace Ubik.Security.Api.Features.Authorizations.Services
{
    public interface IAuthorizationsCommandsService
    {
        public Task<Either<IServiceAndFeatureError, Authorization>> AddAsync(AddAuthorizationCommand authorizationCommand);
        public Task<Either<IServiceAndFeatureError, Authorization>> UpdateAsync(UpdateAuthorizationCommand authorizationCommand);
        public Task<Either<IServiceAndFeatureError, bool>> DeleteAsync(Guid id);
    }
}
