using MassTransit;
using Ubik.Accounting.Structure.Api.Models;

namespace Ubik.Accounting.Structure.Api.Data.Init
{
    internal static class CurrenciesData
    {
        internal static void Load(AccountingDbContext context)
        {
            if (!context.Currencies.Any())
            {
                var baseValuesForTenants = new BaseValuesForTenants();
                var baseValuesForCurrencies = new BaseValuesForCurrencies();

                var currencies = new Currency[]
               {
                    new Currency
                    {
                        Id= baseValuesForCurrencies.CurrencyId1,
                        IsoCode = "CHF",
                        TenantId= baseValuesForTenants.TenantId,
                        Version = NewId.NextGuid()
                    },
                    new Currency
                    {
                        Id= baseValuesForCurrencies.CurrencyId2,
                        IsoCode = "USD",
                        TenantId= baseValuesForTenants.TenantId,
                        Version = NewId.NextGuid()
                    }
               };
                foreach (Currency c in currencies)
                {
                    context.Currencies.Add(c);
                }
                context.SaveChanges();
            }
        }
    }
}
