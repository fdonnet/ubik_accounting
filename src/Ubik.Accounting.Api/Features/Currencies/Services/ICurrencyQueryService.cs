using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Features.Currencies.Services
{
    public interface ICurrencyQueryService
    {
        public Task<IEnumerable<Currency>> GetAllAsync();
    }
}
