using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Accounts.Services;
using Ubik.Accounting.Api.Features.AccountGroups.Services;
using Ubik.Accounting.Api.Features.Classifications.Services;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Features
{
    public class ServiceManager : IServiceManager
    {
        private readonly AccountingContext _context;
        private readonly ICurrentUserService _userService;
        private IAccountService? _accountService;
        private IAccountGroupService? _accountGroupService;
        private IClassificationService? _classificationService;

        public ServiceManager(AccountingContext context, ICurrentUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public IAccountService AccountService
        {
            get
            {
                _accountService ??= new AccountService(_context,_userService);
                return _accountService;
            }
        }

        public IAccountGroupService AccountGroupService
        {
            get
            {
                _accountGroupService ??= new AccountGroupService(_context);
                return _accountGroupService;
            }
        }

        public IClassificationService ClassificationService
        {
            get
            {
                _classificationService ??= new ClassificationService(_context,_userService);
                return _classificationService;
            }
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

    }
}
