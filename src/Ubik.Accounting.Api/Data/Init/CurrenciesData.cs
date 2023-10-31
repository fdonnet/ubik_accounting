using MassTransit;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class CurrenciesData
    {
        internal static void Load(AccountingContext context)
        {
            if (!context.Currencies.Any())
            {
                var baseValuesGeneral = new BaseValuesGeneral();
                var baseValuesForTenants = new BaseValuesForTenants();
                var baseValuesForUsers = new BaseValuesForUsers();
                var baseValuesForCurrencies = new BaseValuesForCurrencies();

                var currencies = new Currency[]
               {
                    new Currency
                    {
                        Id= baseValuesForCurrencies.CurrencyId1,
                        IsoCode = "CHF",
                        CreatedBy= baseValuesForUsers.UserId1,
                        CreatedAt = baseValuesGeneral.GenerationTime,
                        ModifiedBy= baseValuesForUsers.UserId1,
                        ModifiedAt = baseValuesGeneral.GenerationTime,
                        TenantId= baseValuesForTenants.TenantId,
                        Version = NewId.NextGuid()
                    },
                    new Currency
                    {
                        Id= baseValuesForCurrencies.CurrencyId2,
                        IsoCode = "USD",
                        CreatedBy= baseValuesForUsers.UserId1,
                        CreatedAt = baseValuesGeneral.GenerationTime,
                        ModifiedBy= baseValuesForUsers.UserId1,
                        ModifiedAt = baseValuesGeneral.GenerationTime,
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
