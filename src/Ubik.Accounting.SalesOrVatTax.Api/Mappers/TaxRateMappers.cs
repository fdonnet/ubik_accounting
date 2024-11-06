using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Commands;
using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Events;
using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Results;

namespace Ubik.Accounting.SalesOrVatTax.Api.Mappers
{
    public static class TaxRateMappers
    {
        public static IEnumerable<TaxRateStandardResult> ToTaxRateStandardResults(this IEnumerable<TaxRate> current)
        {
            return current.Select(x => new TaxRateStandardResult()
            {
                Id = x.Id,
                ValidFrom = x.ValidFrom,
                ValidTo = x.ValidTo,
                Code = x.Code,
                Description = x.Description,
                Rate = x.Rate,
                Version = x.Version,
            });
        }

        public static TaxRate ToTaxRate(this AddTaxRateCommand current)
        {
            return new TaxRate
            {
                ValidFrom = current.ValidFrom,
                ValidTo = current.ValidTo,
                Code = current.Code,
                Description = current.Description,
                Rate = current.Rate,
            };
        }

        public static TaxRate ToTaxRate(this UpdateTaxRateCommand current)
        {
            return new TaxRate
            {
                Id = current.Id,
                ValidFrom = current.ValidFrom,
                ValidTo = current.ValidTo,
                Code = current.Code,
                Description = current.Description,
                Rate = current.Rate,
                Version = current.Version,
            };
        }

        public static TaxRateAdded ToTaxRateAdded(this TaxRate current)
        {
            return new TaxRateAdded()
            {
                Id = current.Id,
                ValidFrom = current.ValidFrom,
                ValidTo = current.ValidTo,
                Code = current.Code,
                Description = current.Description,
                Rate = current.Rate,
                Version = current.Version,
            };
        }

        public static TaxRateUpdated ToTaxRateUpdated(this TaxRate current)
        {
            return new TaxRateUpdated()
            {
                Id = current.Id,
                ValidFrom = current.ValidFrom,
                ValidTo = current.ValidTo,
                Code = current.Code,
                Description = current.Description,
                Rate = current.Rate,
                Version = current.Version,
            };
        }

        public static TaxRate ToTaxRate(this TaxRate forUpd, TaxRate model)
        {
            model.Id = forUpd.Id;
            model.ValidFrom = forUpd.ValidFrom;
            model.ValidTo = forUpd.ValidTo;
            model.Code = forUpd.Code;
            model.Description = forUpd.Description;
            model.Rate = forUpd.Rate;
            model.Version = forUpd.Version;

            return model;
        }

        public static TaxRateStandardResult ToTaxRateStandardResult(this TaxRate current)
        {
            return new TaxRateStandardResult()
            {
                Id = current.Id,
                ValidFrom = current.ValidFrom,
                ValidTo = current.ValidTo,
                Code = current.Code,
                Description = current.Description,
                Rate = current.Rate,
                Version = current.Version,
            };
        }
    }
}
