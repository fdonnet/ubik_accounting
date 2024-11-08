using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;

namespace Ubik.Accounting.Transaction.Api.Mappers
{
    public static class TxMappers
    {
        public static TxSubmited ToTxSubmited(this SubmitTxCommand current)
        {
            return new TxSubmited
            {
                ValueDate = current.ValueDate,
                Amount = current.Amount,
                Entries = current.Entries.Select(x => x.ToTxEntrySubmited())
            };
        }

        public static TxEntrySubmited ToTxEntrySubmited(this SubmitTxEntry current)
        {
            return new TxEntrySubmited
            {
                Description = current.Description,
                AccountId = current.AccountId,
                Amount = current.Amount,
                AmountAdditionnalInfo = current.AmountAdditionnalInfo?.ToTxEntryAdditionalAmountInfoSubmited(),
                Label = current.Label,
                Sign = current.Sign,
                TaxInfo = current.TaxInfo?.ToTxEntryTaxInfoSubmited(),
                Type = current.Type
            };
        }

        public static TxEntryAdditionalAmountInfoSubmited ToTxEntryAdditionalAmountInfoSubmited(this SubmitTxEntryAdditionalAmountInfo current)
        {
            return new TxEntryAdditionalAmountInfoSubmited
            {
                ExchangeRate = current.ExchangeRate,
                OriginalAmount = current.OriginalAmount,
                OriginalCurrencyId = current.OriginalCurrencyId
            };
        }

        public static TxEntryTaxInfoSubmited ToTxEntryTaxInfoSubmited(this SubmitTxEntryTaxInfo current)
        {
            return new TxEntryTaxInfoSubmited
            {
                TaxAppliedRate = current.TaxAppliedRate,
                TaxRateId = current.TaxRateId
            };
        }
    }
}
