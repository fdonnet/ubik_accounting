using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;

namespace Ubik.Security.Api.Features.RolesAuthorizations.Services
{
    public interface IRolesAuthorizationsCommandsService
    {
        public Task<Either<IFeatureError, RoleAuthorization>> AddAsync(AddRoleAuthorizationCommand command);
        public Task<Either<IFeatureError, bool>> ExecuteDeleteAsync(Guid id);
    }
}
