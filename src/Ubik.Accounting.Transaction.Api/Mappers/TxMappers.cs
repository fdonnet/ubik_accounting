using LanguageExt;
using MassTransit;
using Ubik.Accounting.Transaction.Api.Models;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;
using Ubik.DB.Common.Models;

namespace Ubik.Accounting.Transaction.Api.Mappers
{
    public static class TxMappers
    {

        public static TxSubmitted ToTxSubmitted(this SubmitTxCommand current)
        {
            return new TxSubmitted
            {
                Id = NewId.NextGuid(),
                ValueDate = current.ValueDate,
                Amount = current.Amount,
                Entries = current.Entries.Select(x => x.ToTxEntrySubmitted()),
            };
        }

        public static TxValidated ToTxValidated(this TxSubmitted current)
        {
            return new TxValidated
            {
                Id = NewId.NextGuid(),
                ValueDate = current.ValueDate,
                Amount = current.Amount,
                Entries = current.Entries.Select(x => x.ToTxEntryValidated()),
            };
        }

        public static Tx ToTx(this TxValidated current)
        {
            var now = DateTime.UtcNow;
            return new Tx
            {
                Id = current.Id,
                ValueDate = current.ValueDate,
                Amount = current.Amount,
            };
        }

        public static TxAdded ToTxAdded(this Tx current, IEnumerable<Entry> entries)
        {
            return new TxAdded
            {
                Id = current.Id,
                ValueDate = current.ValueDate,
                Amount = current.Amount,
                Entries = entries.Select(x=>x.ToTxEntryAdded()),
                Version = current.Version
            };
        }

        public static TxEntrySubmitted ToTxEntrySubmitted(this SubmitTxEntry current)
        {
            return new TxEntrySubmitted
            {
                Id = NewId.NextGuid(),
                Description = current.Description,
                AccountId = current.AccountId,
                Amount = current.Amount,
                AmountAdditionnalInfo = current.AmountAdditionnalInfo?.ToTxEntryAdditionalAmountInfoSubmitted(),
                Label = current.Label,
                Sign = current.Sign,
                TaxInfo = current.TaxInfo?.ToTxEntryTaxInfoSubmitted(),
                Type = current.Type
            };
        }

        public static TxEntryValidated ToTxEntryValidated(this TxEntrySubmitted current)
        {
            return new TxEntryValidated
            {
                Id = NewId.NextGuid(),
                Description = current.Description,
                AccountId = current.AccountId,
                Amount = current.Amount,
                AmountAdditionnalInfo = current.AmountAdditionnalInfo?.ToTxEntryAdditionalAmountInfoValidated(),
                Label = current.Label,
                Sign = current.Sign,
                TaxInfo = current.TaxInfo?.ToTxEntryTaxInfoValidated(),
                Type = current.Type
            };
        }

        public static Entry ToEntry(this TxEntryValidated current, Guid txId)
        {
            return new Entry
            {
                Id = current.Id,
                Description = current.Description,
                AccountId = current.AccountId,
                Amount = current.Amount,
                AmountAdditionnalInfo = current.AmountAdditionnalInfo?.ToAmountAdditionnalInfo(),
                Label = current.Label,
                Sign = current.Sign,
                TaxInfo = current.TaxInfo?.ToTaxInfo(),
                Type = current.Type,
                TxId = txId,
            };
        }

        public static TxEntryAdded ToTxEntryAdded(this Entry current)
        {
            return new TxEntryAdded
            {
                Id = current.Id,
                Description = current.Description,
                AccountId = current.AccountId,
                Amount = current.Amount,
                AmountAdditionnalInfo = current.AmountAdditionnalInfo?.ToTxEntryAdditionalAmountInfoAdded(),
                Label = current.Label,
                Sign = current.Sign,
                TaxInfo = current.TaxInfo?.ToTxEntryTaxInfoAdded(),
                Type = current.Type,
                Version = current.Version,
            };
        }

        public static TxEntryAdditionalAmountInfoSubmitted ToTxEntryAdditionalAmountInfoSubmitted(this SubmitTxEntryAdditionalAmountInfo current)
        {
            return new TxEntryAdditionalAmountInfoSubmitted
            {
                ExchangeRate = current.ExchangeRate,
                OriginalAmount = current.OriginalAmount,
                OriginalCurrencyId = current.OriginalCurrencyId
            };
        }

        public static TxEntryAdditionalAmountInfoValidated ToTxEntryAdditionalAmountInfoValidated(this TxEntryAdditionalAmountInfoSubmitted current)
        {
            return new TxEntryAdditionalAmountInfoValidated
            {
                ExchangeRate = current.ExchangeRate,
                OriginalAmount = current.OriginalAmount,
                OriginalCurrencyId = current.OriginalCurrencyId
            };
        }

        public static AmountAdditionalInfo ToAmountAdditionnalInfo(this TxEntryAdditionalAmountInfoValidated current)
        {
            return new AmountAdditionalInfo(current.OriginalAmount, current.OriginalCurrencyId, current.ExchangeRate);
        }

        public static TxEntryAdditionalAmountInfoAdded ToTxEntryAdditionalAmountInfoAdded(this AmountAdditionalInfo current)
        {
            return new TxEntryAdditionalAmountInfoAdded
            {
                ExchangeRate = current.ExchangeRate,
                OriginalAmount = current.OriginalAmount,
                OriginalCurrencyId = current.OriginalCurrencyId,
            };
        }

        public static TxEntryTaxInfoSubmitted ToTxEntryTaxInfoSubmitted(this SubmitTxEntryTaxInfo current)
        {
            return new TxEntryTaxInfoSubmitted
            {
                TaxAppliedRate = current.TaxAppliedRate,
                TaxRateId = current.TaxRateId
            };
        }

        public static TxEntryTaxInfoValidated ToTxEntryTaxInfoValidated(this TxEntryTaxInfoSubmitted current)
        {
            return new TxEntryTaxInfoValidated
            {
                TaxAppliedRate = current.TaxAppliedRate,
                TaxRateId = current.TaxRateId
            };
        }

        public static TaxInfo ToTaxInfo(this TxEntryTaxInfoValidated current)
        {
            return new TaxInfo(current.TaxAppliedRate, current.TaxRateId);
        }

        public static TxEntryTaxInfoAdded ToTxEntryTaxInfoAdded(this TaxInfo current)
        {
            return new TxEntryTaxInfoAdded
            {
                TaxAppliedRate = current.TaxAppliedRate,
                TaxRateId = current.TaxRateId
            };
        }
    }
}
