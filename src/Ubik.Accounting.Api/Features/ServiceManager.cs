using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Accounts.Services;
using Ubik.Accounting.Api.Features.AccountGroups.Services;
using Ubik.Accounting.Api.Features.Classifications.Services;
using Ubik.ApiService.Common.Services;
using Ubik.Accounting.Api.Features.Currencies.Services;

namespace Ubik.Accounting.Api.Features
{
    public class ServiceManager : IServiceManager
    {
        private readonly AccountingDbContext _context;
        private readonly ICurrentUserService _userService;
        private IAccountService? _accountService;
        private IAccountGroupService? _accountGroupService;
        private IClassificationService? _classificationService;
        private ICurrencyService? _currencyService;

        public ServiceManager(AccountingDbContext context, ICurrentUserService userService)
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
                _accountGroupService ??= new AccountGroupService(_context,_userService);
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

        public ICurrencyService CurrencyService
        {
            get
            {
                _currencyService ??= new CurrencyService(_context);
                return _currencyService;
            }
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

    }
}
