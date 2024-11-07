using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Transaction.Contracts.Entries.Enums;

namespace Ubik.Accounting.Transaction.Contracts.Txs.Commands
{
    public record SubmitTxCommand
    {
        public Guid Id { get; init; }
        public required DateOnly ValueDate { get; init; }
        public required string Label { get; init; }
        public decimal Amount { get; init; }
        public IEnumerable<TxEntry> Entries { get; init; } = default!;
    }

    public record TxEntry
    {
        public required EntryType Type { get; init; }
        public required DebitCredit Sign { get; init; }
        public required Guid AccountId { get; init; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public required decimal Amount { get; set; }
        public TxEntryAdditionalAmountInfo? AmountAdditionnalInfo { get; set; }
        public TxEntryTaxInfo? TaxInfo { get; set; } = default!;
    }

    public record TxEntryAdditionalAmountInfo
    {
        public decimal OriginalAmount { get; init; }
        public Guid OriginalCurrencyId { get; init; }
        public decimal ExchangeRate { get; init; }
    }

    public record TxEntryTaxInfo
    {
        public decimal TaxAppliedRate { get; init; }
        public Guid TaxRateId { get; init; }
    }
}
