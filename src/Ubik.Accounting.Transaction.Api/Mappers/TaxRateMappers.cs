using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Events;
using Ubik.Accounting.Transaction.Api.Models;

namespace Ubik.Accounting.Transaction.Api.Mappers
{
    public static class TaxRateMappers
    {
        public static TaxRate ToTaxRate(this TaxRateAdded current)
        {
            return new TaxRate
            {
                Code = current.Code,
                Id = current.Id,
                Rate = current.Rate,
                TenantId = current.TenantId,
                Version = current.Version
            };
        }
    }
}
