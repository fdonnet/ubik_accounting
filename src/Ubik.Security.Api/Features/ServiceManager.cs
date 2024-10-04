using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Authorizations.Services;
using IAuthorizationService = Ubik.Security.Api.Features.Authorizations.Services.IAuthorizationService;

namespace Ubik.Security.Api.Features
{
    public class ServiceManager(SecurityDbContext context, ICurrentUserService userService) : IServiceManager
    {
        private readonly SecurityDbContext _context = context;
        private readonly ICurrentUserService _userService = userService;

        private IAuthorizationService _authorizationService = default!;


        public IAuthorizationService AuthorizationService
        {
            get
            {
                _authorizationService ??= new AuthorizationService(_context, _userService);
                return _authorizationService;
            }
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
