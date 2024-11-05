using MassTransit;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Commands;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Events;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Results;

namespace Ubik.Accounting.SalesOrVatTax.Api.Mappers
{
    public static class AccountTaxRateConfigMappers
    {
        public static IEnumerable<AccountTaxRateConfigStandardResult> ToAccountTaxRateConfigStandardResults(this IEnumerable<AccountTaxRateConfig> current)
        {
            return current.Select(x => new AccountTaxRateConfigStandardResult()
            {
                Id = x.Id,
                AccountId = x.AccountId,
                TaxRateId = x.TaxRateId,
                TaxAccountId = x.TaxAccountId,
                Version = x.Version,
            });
        }

        public static AccountTaxRateConfig ToAccountTaxRateConfig(this AddAccountTaxRateConfigCommand current)
        {
            return new AccountTaxRateConfig
            {
                Id = NewId.NextGuid(),
                AccountId = current.AccountId,
                TaxRateId = current.TaxRateId,
                TaxAccountId = current.TaxAccountId,
                Version = NewId.NextGuid()
            };
        }

        public static AccountTaxRateConfigAdded ToAccountTaxRateConfigAdded(this AccountTaxRateConfig current)
        {
            return new AccountTaxRateConfigAdded
            {
                Id = current.Id,
                AccountId = current.AccountId,
                TaxRateId = current.TaxRateId,
                TaxAccountId = current.TaxAccountId,
                Version = current.Version,
            };
        }

        public static AccountTaxRateConfigStandardResult ToAccountTaxRateConfigStandardResult(this AccountTaxRateConfig current)
        {
            return new AccountTaxRateConfigStandardResult
            {
                Id = current.Id,
                AccountId = current.AccountId,
                TaxRateId = current.TaxRateId,
                TaxAccountId = current.TaxAccountId,
                Version = current.Version,
            };
        }
    }
}
