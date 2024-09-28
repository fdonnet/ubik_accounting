using Ubik.Security.Api.Features.Authorizations.Services;
using Ubik.Security.Api.Features.Users.Services;

namespace Ubik.Security.Api.Features
{

    public interface IServiceManager
    {
        IUserManagementService UserManagementService { get; }
        IAuthorizationService AuthorizationService { get; }
        Task SaveAsync();
    }
}
