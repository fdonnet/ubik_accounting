using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.Currencies.Services
{
    public interface ICurrencyService
    {
        public Task<IEnumerable<Currency>> GetAllAsync();
    }
}
