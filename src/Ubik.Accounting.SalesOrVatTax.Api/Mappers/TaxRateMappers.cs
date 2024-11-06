using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Commands;
using Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Events;
using Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Results;

namespace Ubik.Accounting.SalesOrVatTax.Api.Mappers
{
    public static class TaxRateMappers
    {
        public static IEnumerable<SalesOrVatTaxRateStandardResult> ToSalesOrVatTaxRateStandardResults(this IEnumerable<TaxRate> current)
        {
            return current.Select(x => new SalesOrVatTaxRateStandardResult()
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

        public static TaxRate ToSalesOrVatTaxRate(this TaxRateCommand current)
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

        public static TaxRate ToSalesOrVatTaxRate(this UpdateSalesOrVatTaxRateCommand current)
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

        public static SalesOrVatTaxRateAdded ToSalesOrVatTaxRateAdded(this TaxRate current)
        {
            return new SalesOrVatTaxRateAdded()
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

        public static SalesOrVatTaxRateUpdated ToSalesOrVatTaxRateUpdated(this TaxRate current)
        {
            return new SalesOrVatTaxRateUpdated()
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

        public static TaxRate ToSalesOrVatTaxRate(this TaxRate forUpd, TaxRate model)
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

        public static SalesOrVatTaxRateStandardResult ToSalesOrVatTaxRateStandardResult(this TaxRate current)
        {
            return new SalesOrVatTaxRateStandardResult()
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
