using Ubik.Security.Api.Features.Authorizations.Services;

namespace Ubik.Security.Api.Features
{

    public interface IServiceManager
    {
        IAuthorizationService AuthorizationService { get; }
        Task SaveAsync();
    }
}
