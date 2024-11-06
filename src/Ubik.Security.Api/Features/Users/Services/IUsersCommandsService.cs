using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Users.Commands;

namespace Ubik.Security.Api.Features.Users.Services
{
    public interface IUsersCommandsService
    {
        Task<Either<IFeatureError, User>> AddAsync(AddUserCommand userCommand);
        Task<Either<IFeatureError, Tenant>> AddNewTenantAsync(Guid userId,AddTenantCommand command);
        Task<Either<IFeatureError, Role>> AddRoleInTenantAsync(Guid userId, Guid roleId);
    }
}
