using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Currencies.Results;

namespace Ubik.Accounting.Api.Mappers
{
    public static class CurrencyMappers
    {
        public static IEnumerable<CurrencyStandardResult> ToCurrencyStandardResults(this IEnumerable<Currency> current)
        {
            return current.Select(x => new CurrencyStandardResult()
            {
                Id = x.Id,
                IsoCode = x.IsoCode,
                Version = x.Version
            });
        }
    }
}
