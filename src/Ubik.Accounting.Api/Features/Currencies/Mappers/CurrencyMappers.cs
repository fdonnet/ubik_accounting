using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Currencies.Results;

namespace Ubik.Accounting.Api.Features.Currencies.Mappers
{
    public static class CurrencyMappers
    {
        public static IEnumerable<GetAllCurrenciesResult> ToGetAllCurrenciesResult(this IEnumerable<Currency> current)
        {
            return current.Select(x => new GetAllCurrenciesResult()
            {
                Id = x.Id,
                IsoCode = x.IsoCode,
                Version = x.Version
            });
        }
    }
}
