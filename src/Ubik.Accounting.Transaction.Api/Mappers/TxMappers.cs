using LanguageExt;
using MassTransit;
using MassTransit.Caching.Internals;
using Ubik.Accounting.Transaction.Api.Models;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Enums;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;
using Ubik.DB.Common.Models;

namespace Ubik.Accounting.Transaction.Api.Mappers
{
    public static class TxMappers
    {

        public static TxSubmitted ToTxSubmitted(this Tx current, IEnumerable<Entry> entries)
        {
            return new TxSubmitted
            {
                Id = current.Id,
                ValueDate = current.ValueDate,
                Amount = current.Amount,
                Entries = entries.Select(e => e.ToTxEntrySubmitted()),
                Version = current.Version,
                State = current.State.ToTxTxStateInfoSubmitted()
            };
        }

        public static TxValidated ToTxValidated(this TxSubmitted current)
        {
            return new TxValidated
            {
                Id = current.Id,
                Version = current.Version
            };
        }

        public static Tx ToTx(this SubmitTxCommand current)
        {
            var now = DateTime.UtcNow;
            return new Tx
            {
                Id = NewId.NextGuid(),
                ValueDate = current.ValueDate,
                Amount = current.Amount,
                State = new TxStateInfo
                {
                    State = TxState.Submitted,
                    Reason = null
                }
            };
        }

        public static TxEntrySubmitted ToTxEntrySubmitted(this Entry current)
        {
            return new TxEntrySubmitted
            {
                Id = current.Id,
                Description = current.Description,
                AccountId = current.AccountId,
                Amount = current.Amount,
                AmountAdditionnalInfo = current.AmountAdditionnalInfo?.ToTxEntryAdditionalAmountInfoSubmitted(),
                Label = current.Label,
                Sign = current.Sign,
                Version = current.Version,
                TaxInfo = current.TaxInfo?.ToTxEntryTaxInfoSubmitted(),
                Type = current.Type
            };
        }

        public static Entry ToEntry(this SubmitTxEntry current, Guid txId)
        {
            return new Entry
            {
                Id = NewId.NextGuid(),
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

        public static TxEntryAdditionalAmountInfoSubmitted ToTxEntryAdditionalAmountInfoSubmitted(this AmountAdditionalInfo current)
        {
            return new TxEntryAdditionalAmountInfoSubmitted
            {
                ExchangeRate = current.ExchangeRate,
                OriginalAmount = current.OriginalAmount,
                OriginalCurrencyId = current.OriginalCurrencyId
            };
        }

        public static AmountAdditionalInfo ToAmountAdditionnalInfo(this SubmitTxEntryAdditionalAmountInfo current)
        {
            return new AmountAdditionalInfo(current.OriginalAmount, current.OriginalCurrencyId, current.ExchangeRate);
        }

        public static TxEntryTaxInfoSubmitted ToTxEntryTaxInfoSubmitted(this TaxInfo current)
        {
            return new TxEntryTaxInfoSubmitted
            {
                TaxAppliedRate = current.TaxAppliedRate,
                TaxRateId = current.TaxRateId
            };
        }

        public static TxStateInfoSubmitted ToTxTxStateInfoSubmitted(this TxStateInfo current)
        {
            return new TxStateInfoSubmitted
            {
                State = current.State,
                Reason = current.Reason
            };
        }

        public static TaxInfo ToTaxInfo(this SubmitTxEntryTaxInfo current)
        {
            return new TaxInfo(current.TaxAppliedRate, current.TaxRateId);
        }

    }
}
