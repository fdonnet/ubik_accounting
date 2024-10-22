using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.Accounts.Services;
using Ubik.Accounting.Api.Features.AccountGroups.Services;
using Ubik.Accounting.Api.Features.Classifications.Services;
using Ubik.ApiService.Common.Services;
using Ubik.Accounting.Api.Features.Currencies.Services;

namespace Ubik.Accounting.Api.Features
{
    public class ServiceManager(AccountingDbContext context, ICurrentUser currentUser) : IServiceManager
    {
        private readonly AccountingDbContext _context = context;
        private readonly ICurrentUser _currentUser = currentUser;
        private ICurrencyService? _currencyService;

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
