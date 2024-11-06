using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Roles.Commands;

namespace Ubik.Security.Api.Features.Roles.Services
{
    public interface IRolesAdminCommandsService
    {
        public Task<Either<IFeatureError, Role>> AddAsync(AddRoleCommand authorizationCommand);
        public Task<Either<IFeatureError, Role>> UpdateAsync(UpdateRoleCommand authorizationCommand);
        public Task<Either<IFeatureError, bool>> DeleteAsync(Guid id);
    }
}
