using Ubik.Security.Api.Features.Users.Services;

namespace Ubik.Security.Api.Features
{

    public interface IServiceManager
    {
        IUserManagementService UserManagementService { get; }
        Task SaveAsync();
    }
}
