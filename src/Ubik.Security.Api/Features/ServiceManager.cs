using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Users.Services;

namespace Ubik.Security.Api.Features
{
    public class ServiceManager(SecurityDbContext context, ICurrentUserService userService) : IServiceManager
    {
        private readonly SecurityDbContext _context = context;
        private readonly ICurrentUserService _userService = userService;

        private IUserManagementService _userManagementService = default!;

        public IUserManagementService UserManagementService
        {
            get
            {
                _userManagementService ??= new UserManagementService(_context,_userService);
                return _userManagementService;
            }
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
