using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.VatRate.Commands;
using Ubik.Accounting.Structure.Contracts.VatRate.Events;
using Ubik.Accounting.Structure.Contracts.VatRate.Results;

namespace Ubik.Accounting.Structure.Api.Mappers
{
    public static class VatRateMappers
    {
        public static IEnumerable<VatRateStandardResult> ToVatRateStandardResults(this IEnumerable<VatRate> current)
        {
            return current.Select(x => new VatRateStandardResult()
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

        public static VatRate ToVatRate(this AddVatRateCommand current)
        {
            return new VatRate
            {
                ValidFrom = current.ValidFrom,
                ValidTo = current.ValidTo,
                Code = current.Code,
                Description = current.Description,
                Rate = current.Rate,
            };
        }

        public static VatRate ToVatRate(this UpdateVatRateCommand current)
        {
            return new VatRate
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

        public static VatRateAdded ToVatRateAdded(this VatRate current)
        {
            return new VatRateAdded()
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

        public static VatRateUpdated ToVatRateUpdated(this VatRate current)
        {
            return new VatRateUpdated()
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

        public static VatRate ToVatRate(this VatRate forUpd, VatRate model)
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

        public static VatRateStandardResult ToVatRateStandardResult(this VatRate current)
        {
            return new VatRateStandardResult()
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
