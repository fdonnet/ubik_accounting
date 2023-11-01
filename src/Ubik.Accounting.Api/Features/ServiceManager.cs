using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Classifications;
using Ubik.Accounting.Api.Features.AccountGroups;
using Ubik.Accounting.Api.Features.Accounts.Services;

namespace Ubik.Accounting.Api.Features
{
    public class ServiceManager : IServiceManager
    {
        private readonly AccountingContext _context;
        private IAccountService? _accountService;
        private IAccountGroupService? _accountGroupService;
        private IClassificationService? _classificationService;

        public ServiceManager(AccountingContext context)
        {
            _context = context;
        }

        public IAccountService AccountService
        {
            get
            {
                _accountService ??= new AccountService(_context);
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
                _classificationService ??= new ClassificationService(_context);
                return _classificationService;
            }
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
