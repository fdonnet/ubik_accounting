using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Accounts;

namespace Ubik.Accounting.Api.Features
{
    public class ServiceManager : IServiceManager
    {
        private readonly AccountingContext _context;
        private IAccountService? _accountService;

        public ServiceManager(AccountingContext context)
        {
            _context = context;
        }

        public IAccountService AccountService
        {
            get
            {
                if (_accountService == null)
                    _accountService = new AccountService(_context);
                return _accountService;
            }
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
