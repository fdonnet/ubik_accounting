using Ubik.Accounting.Structure.Api.Models;

namespace Ubik.Accounting.Structure.Api.Features.Currencies.Services
{
    public interface ICurrencyQueryService
    {
        public Task<IEnumerable<Currency>> GetAllAsync();
    }
}
